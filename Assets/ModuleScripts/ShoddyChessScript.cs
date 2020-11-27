using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using KModkit;
using ShoddyChess;
using UnityEngine;
using UnityEngine.Serialization;
using rnd = UnityEngine.Random;

public partial class ShoddyChessScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Info;
    public KMBombModule Module;
    public KMBossModule BossModule;

    //Information phase
    public GameObject InformationPhase;
    public TextMesh InformationText;
    public TextMesh StageText;
    public KMSelectable InformationButton;
    public Animator DoorAnimation;
    public Animator SmallDoorAnimation;
    public GameObject SolvingPhase;
    public KMSelectable[] PieceSwitcherButtons; //0 left, 1 right, 2 display.
    public MeshRenderer ScreenRenderer;
    public Material[] ScreenMats; //0 white, 1 black, 2 clear
    public TextMesh ScreenText;

    //Don't judge me....
    [FormerlySerializedAs("Peices")] public GameObject[] Pieces;
    [FormerlySerializedAs("PeiceMats")] public Material[] PieceMats;

    public AudioClip[] Sounds; //0 move, 1 remove, 2 stage

    private bool _isSolved;
    private bool _animating;

    private bool _interactable
    {
        get { return !(_isSolved || _animating); }
    }

    private static int _moduleIdCounter = 1;
    private int _moduleId;

    private int _stage;

    private string[] _ignored;

    private readonly ChessPiece[,] _chessBoard = Constants.CreateBoard();
    private readonly BoardItem[,] _inputtedBoard = new BoardItem[8, 8];
    private BoardItem CurrentItem;
    private readonly List<string> _stages = new List<string>();
    private bool _clearMode;
    private bool _solvable;

    private MoveQueue _whiteMoveQueue;
    private MoveQueue _blackMoveQueue;

    private bool _whiteKingDead;
    private bool _blackKingDead;

    private bool _isInSolvingPhase;
    private bool _isShowingStages;
    private Coroutine _showStagesCoroutine;
    private Coroutine _buttonHold;
    private bool _holding;

    private bool _blackPlaying;

    private int _buffer;

    // [UnityEditor.MenuItem("DoStuff/DoStuff")]
    // public static void DoStuff()
    // {
    // 	Debug.LogFormat("ok");
    // 	var m = FindObjectOfType<ShoddyChessScript>();
    // 	// for (var i = 0; i < m.pieces.Length; i++)
    // 	// {
    // 	// 	var highlight = m.pieces[i].GetComponentInParent<KMSelectable>().GetComponentInChildren<KMHighlightable>().gameObject;
    // 	// 	highlight.transform.localScale = new Vector3(.9f, .5f, .9f);
    // 	// }
    //     for (int i = 0; i < m.pieces.Length; i++)
    //     {
    //         var text = m.pieces[i].GetComponentInChildren<TextMesh>();
    //         text.font = m.ok;
    //     }
    // }

    // this entire code and everything was made by livio maskram is holding me hostage and not looking at the code pls help me if you see this send german sausage sandwiches to 23 ikea lane in sweden city 

    void Start()
    {
        DoorAnimation.gameObject.SetActive(true);
        SmallDoorAnimation.gameObject.SetActive(true);
        InformationPhase.SetActive(true);
        SolvingPhase.SetActive(true);
        _moduleId = _moduleIdCounter++;
        _ignored = BossModule.GetIgnoredModules(Module, new[]
        {
            "14",
            "42",
            "501",
            "A>N<D",
            "Bamboozling Time Keeper",
            "Brainf---",
            "Busy Beaver",
            "Don't Touch Anything",
            "Forget Any Color",
            "Forget Enigma",
            "Forget Everything",
            "Forget It Not",
            "Forget Me Later",
            "Forget Me Not",
            "Forget Perspective",
            "Forget The Colors",
            "Forget Them All",
            "Forget This",
            "Forget Us Not",
            "Iconic",
            "Keypad Directionality",
            "Kugelblitz",
            "Multitask",
            "OmegaForget",
            "Organization",
            "Password Destroyer",
            "Purgatory",
            "RPS Judging",
            "Security Council",
            "Shoddy Chess",
            "Simon Forgets",
            "Simon's Stages",
            "Souvenir",
            "Tallordered Keys",
            "The Time Keeper",
            "The Troll",
            "The Twin",
            "The Very Annoying Button",
            "Timing is Everything",
            "Turn The Key",
            "Ultimate Custom Night",
            "Übermodule",
            "Whiteout"
        });

        if (Info.GetSolvableModuleNames().Count(x => !_ignored.Contains(x)) < 2)
        {
            LogMessage("Not enough solvable modules, automatically solving module.");
            Module.HandlePass();

            _isSolved = true;
            InformationText.text = "--";
        }

        CurrentItem = new BoardItem();

        for (int i = 0; i < 3; i++)
        {
            var j = i;
            PieceSwitcherButtons[j].OnInteract += delegate
            {
                if (!_interactable)
                {
                    return false;
                }

                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
                PieceSwitcherButtons[j].AddInteractionPunch(.25f);
                switch (j)
                {
                    case 0: //left
                        if (_clearMode)
                        {
                            return false;
                        }

                        if (CurrentItem.Id == PieceIdentifier.Q)
                        {
                            CurrentItem.Id = PieceIdentifier.P8;
                        }
                        else
                        {
                            CurrentItem.Id--;
                        }

                        ScreenText.text = Constants._symbols[(int) CurrentItem.Id];
                        ScreenText.fontSize = (CurrentItem.Id == PieceIdentifier.Q) ? 100 : 65;
                        break;
                    case 1: //right
                        if (_clearMode)
                        {
                            return false;
                        }

                        if (CurrentItem.Id == PieceIdentifier.P8)
                        {
                            CurrentItem.Id = PieceIdentifier.Q;
                        }
                        else
                        {
                            CurrentItem.Id++;
                        }

                        ScreenText.text = Constants._symbols[(int) CurrentItem.Id];
                        ScreenText.fontSize = (CurrentItem.Id == PieceIdentifier.Q) ? 100 : 65;
                        break;

                    case 2: //display
                        _clearMode = false;
                        if (_buttonHold != null)
                        {
                            _holding = false;
                            StopCoroutine(_buttonHold);
                            _buttonHold = null;
                        }

                        _buttonHold = StartCoroutine(HoldChecker());

                        break;
                }

                return false;
            };
        }

        PieceSwitcherButtons[2].OnInteractEnded += delegate
        {
            if (!_interactable)
            {
                return;
            }
            
            PieceSwitcherButtons[2].AddInteractionPunch(.25f);

            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, Module.transform);
            StopCoroutine(_buttonHold);
            if (!_holding)
            {
                ScreenText.text = Constants._symbols[(int) CurrentItem.Id];
                if (CurrentItem.Color == PieceColor.White)
                {
                    CurrentItem.Color = PieceColor.Black;
                    ScreenText.color = Color.white;
                    ScreenRenderer.material = ScreenMats[1];
                    return;
                }

                CurrentItem.Color = PieceColor.White;
                ScreenText.color = Color.black;
                ScreenRenderer.material = ScreenMats[0];
            }
        };

        InformationButton.OnInteract += delegate
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, InformationButton.transform);
            InformationButton.AddInteractionPunch();
            if (!_interactable)
            {
                return false;
            }

            if (_isShowingStages)
            {
                StopCoroutine(_showStagesCoroutine);
                _showStagesCoroutine = null;
                _isInSolvingPhase = true;
                StartCoroutine(ChangeState(true));
                _isShowingStages = false;
                return false;
            }

            if (_whiteKingDead || _blackKingDead)
            {
                _isInSolvingPhase = true;
                _solvable = true;
                StartCoroutine(ChangeState(true));
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (_chessBoard[y, x] == null)
                        {
                            continue;
                        }

                        if (_chessBoard[y, x].Id == PieceIdentifier.K)
                        {
                            PlacePiece(
                                new BoardItem(PieceIdentifier.K,
                                    _chessBoard[y, x].Color == PieceColor.White ? PieceColor.White : PieceColor.Black,
                                    false), y, x);
                        }
                    }
                }

                return false;
            }

            Module.HandleStrike();
            LogMessage("The !!! button was pressed while a king was not dead. Strike!");
            return false;
        };

        for (int i = 0; i < Pieces.Length; i++)
        {
            var j = i;
            var selectable = Pieces[j].GetComponentInParent<KMSelectable>();
            selectable.OnInteract += delegate
            {
                HandlePress(j);
                return false;
            };
            Pieces[j].SetActive(false);
        }

        InformationPhase.SetActive(true);
        SolvingPhase.SetActive(false);
        Module.OnActivate += Activate;
    }

    private void Activate()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSequenceMechanism, Module.transform);
        StartAnimation(true);
        if (!_isSolved)
        {
            GenerateInitialStage();
        }
    }

    private void StartAnimation(bool isopen)
    {
        if (isopen)
        {
            DoorAnimation.ResetTrigger(Constants.Close);
            DoorAnimation.SetTrigger(Constants.Open);
            SmallDoorAnimation.ResetTrigger(Constants.SmallClose);
            SmallDoorAnimation.SetTrigger(Constants.SmallOpen);
        }
        else
        {
            DoorAnimation.ResetTrigger(Constants.Open);
            DoorAnimation.SetTrigger(Constants.Close);
            SmallDoorAnimation.ResetTrigger(Constants.SmallOpen);
            SmallDoorAnimation.SetTrigger(Constants.SmallClose);
        }
    }

    private IEnumerator SolveHandler()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSequenceMechanism, Module.transform);
        StartAnimation(false);
        yield return new WaitForSeconds(1f);
        InformationPhase.SetActive(false);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
        SolvingPhase.SetActive(false);
    }

    private IEnumerator ChangeState(bool changeToSolvingPhase)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSequenceMechanism, Module.transform);
        _animating = true;
        StartAnimation(false);
        yield return new WaitForSeconds(1f);
        if (changeToSolvingPhase)
        {
            InformationPhase.SetActive(false);
            SolvingPhase.SetActive(true);
        }
        else
        {
            InformationPhase.SetActive(true);
            SolvingPhase.SetActive(false);
        }

        StartAnimation(true);
        yield return new WaitForSeconds(.5f);
        _animating = false;
    }

    private bool CheckAnswer(ChessPiece[,] finalBoard, BoardItem[,] input)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (finalBoard[y, x] == null && input[y, x] == null)
                {
                    continue;
                }

                if (finalBoard[y, x] == null || input[y, x] == null)
                {
                    LogMessage(
                        "Error found at {0}, an empty square was found on one grid but not on the other!",ShoddyChessHelper.GetCoordinate(y,x));
                    return false;
                }

                if ((finalBoard[y, x].Id != input[y, x].Id) || (finalBoard[y, x].Color != input[y, x].Color))
                {
                    LogMessage("Error found at {0}, input was {1}, when I expected {2}!", ShoddyChessHelper.GetCoordinate(y,x), input[y, x].ToString(),
                        finalBoard[y, x].ToString());
                    return false;
                }
            }
        }

        return true;
    }

    private void HandlePress(int i)
    {
        if (!_interactable)
        {
            return;
        }

        var coor = ShoddyChessHelper.GetCoordinateFromIndex(i);
        var item = _inputtedBoard[coor.Item1, coor.Item2];

        if (item != null)
        {
            if (!item.CanBeChanged && item.Id != PieceIdentifier.K)
            {
                return;
            }

            if (item.Id == PieceIdentifier.K)
            {
                LogMessage("The submitted answer was:");
                LogBoard(_inputtedBoard);
                if (CheckAnswer(_chessBoard, _inputtedBoard))
                {
                    LogMessage("That matches the final board. Module solved!");
                    StartCoroutine(SolveHandler());
                    _isSolved = true;
                    Module.HandlePass();
                    return;
                }

                Module.HandleStrike();
                LogMessage("The submitted board does not match the final board:");
                LogBoard(_chessBoard);
                LogMessage("Strike!");
                _showStagesCoroutine = StartCoroutine(ShowStages());
                return;
            }
        }

        if (_clearMode)
        {
            if (item == null || !item.CanBeChanged)
            {
                return;
            }

            Pieces[i].GetComponentInParent<KMSelectable>().AddInteractionPunch(.15f);
            Audio.PlaySoundAtTransform(Sounds[1].name, Module.transform);
            _inputtedBoard[coor.Item1, coor.Item2] = null;
            Pieces[i].SetActive(false);
            return;
        }

        Pieces[i].GetComponentInParent<KMSelectable>().AddInteractionPunch(.15f);
        Audio.PlaySoundAtTransform(Sounds[0].name, Module.transform);
        PlacePiece(new BoardItem(CurrentItem.Id, CurrentItem.Color, true), coor.Item1, coor.Item2);

        if (CurrentItem.Color == PieceColor.Black)
        {
            Pieces[i].GetComponentInChildren<TextMesh>().color = Color.white;
            Pieces[i].GetComponent<MeshRenderer>().material = PieceMats[1];
        }
        else
        {
            Pieces[i].GetComponentInChildren<TextMesh>().color = Color.black;
            Pieces[i].GetComponent<MeshRenderer>().material = PieceMats[0];
        }
    }

    private void ChangeStageText(int stage)
    {
        var text = "----" + stage.ToString();
        StageText.text = text.Substring(text.Length - 4);
    }

    private void GenerateInitialStage()
    {
        var displayedNumber = rnd.Range(0, 64);
        _stages.Add(displayedNumber.ToString("D2"));
        InformationText.text = displayedNumber.ToString("D2");

        var coordinates = ShoddyChessHelper.GetCoordinateFromIndex(displayedNumber);
        var c = ShoddyChessHelper.GetCoordinate(coordinates.Item1, coordinates.Item2);
        var startingNumber = (char.ToUpper(c[0]) - 64) + int.Parse(c[1].ToString());

        _blackPlaying = startingNumber % 2 != 0;

        var number = startingNumber;

        number += (Info.GetBatteryCount() * 7);
        if (Info.GetModuleIDs().Any(x => x.EqualsAny("ChessModule", "lousyChess")))
        {
            number *= 2;
        }

        number %= 10;

        var whiteMoveQueue = number;

        number = startingNumber;
        Info.GetSerialNumber().ForEach(x => number += Constants._base36string.IndexOf(x));

        number %= 10;
        var blackMoveQueue = number;

        if (whiteMoveQueue == blackMoveQueue)
        {
            blackMoveQueue++;
            blackMoveQueue %= 10;
        }
        
        _whiteMoveQueue = new MoveQueue(whiteMoveQueue);
        LogMessage("The white movequeue is {0}", whiteMoveQueue);
        _blackMoveQueue = new MoveQueue(blackMoveQueue);
        LogMessage("The black movequeue is {0}", blackMoveQueue);
    }

    private void GenerateStage()
    {
        if (_whiteKingDead || _blackKingDead)
        {
            if (_solvable)
            {
                return;
            }

            LogMessage("A module was solved while the {0} king was dead. Strike!", _whiteKingDead ? "white" : "black");
            Module.HandleStrike();
        }
        else if (_stage == Info.GetSolvableModuleNames().Count(x => !_ignored.Contains(x)))
        {
            LogMessage("Module is now in the solve phase, good luck!");
            _isInSolvingPhase = true;
            _solvable = true;
            StartCoroutine(ChangeState(true));
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (_chessBoard[y, x] == null)
                    {
                        continue;
                    }

                    if (_chessBoard[y, x].Id == PieceIdentifier.K || !_chessBoard[y, x].HasMoved)
                    {
                        PlacePiece(
                            new BoardItem(_chessBoard[y, x].Id,
                                _chessBoard[y, x].Color == PieceColor.White ? PieceColor.White : PieceColor.Black,
                                false), y, x);
                    }
                }
            }
        }
        else
        {
            LogMessage("Stage {0}:", _stage);
            Audio.PlaySoundAtTransform(Sounds[2].name, Module.transform);
            ChangeStageText(_stage);

            while (true)
            {
                retry:
                for (var x = 0; x < 8; x++)
                {
                    for (var y = 0; y < 8; y++)
                    {
                        if (_chessBoard[y, x] == null)
                        {
                            continue;
                        }

                        var currentMoveQueuePiece = _blackPlaying ? _blackMoveQueue.CurrentPiece : _whiteMoveQueue.CurrentPiece;
                        var currentMoveQueueColor = _blackPlaying ? PieceColor.Black : PieceColor.White;

                        if (_chessBoard[y, x].Id == currentMoveQueuePiece &&
                            _chessBoard[y, x].Color == currentMoveQueueColor)
                        {
                            //This is the piece
                            var piece = _chessBoard[y, x];
                            var possibleMoves = piece.Handler.GetAvailableMoves(_chessBoard, x, y);
                            if (piece.PawnFirstMove)
                            {
                                piece.PawnFirstMove = false;
                            }

                            if (possibleMoves == null)
                            {
                                //This piece cannot move
                                LogMessage("{0} {1} cannot move, advancing to the next position in the queue",
                                    currentMoveQueueColor.ToString(), currentMoveQueuePiece.ToString());

                                if (_blackPlaying)
                                {
                                    _blackMoveQueue.Advance();
                                }
                                else
                                {
                                    _whiteMoveQueue.Advance();
                                }

                                goto retry;
                            }

                            var move = GetMove(possibleMoves, _chessBoard);
                            
                            LogMessage("{0} {1}, moved from: {2}, to {3}.",
                                currentMoveQueueColor.ToString(), currentMoveQueuePiece.ToString(),
                                ShoddyChessHelper.GetCoordinate(move.From.Item1, move.From.Item2),
                                ShoddyChessHelper.GetCoordinate(move.To.Item1, move.To.Item2));

                            InformationText.text = ShoddyChessHelper.GetCoordinate(move.To.Item1, move.To.Item2);
                            _stages.Add(ShoddyChessHelper.GetCoordinate(move.To.Item1, move.To.Item2));

                            var currentPiece = _chessBoard[move.From.Item1, move.From.Item2];
                            _chessBoard[move.From.Item1, move.From.Item2] = null;
                            currentPiece.HasMoved = true;
                            var pieceTaken = _chessBoard[move.To.Item1, move.To.Item2];

                            if (currentPiece.PieceType == PieceType.Pawn && move.To.Item1 == (currentMoveQueueColor == PieceColor.Black ? 7 : 0))
                            {
                                LogMessage(
                                    "{0} {1} has reached the opposite end of the board and will now act as a queen.",
                                    currentMoveQueueColor.ToString(), currentPiece.Id.ToString());
                                currentPiece.PieceType = PieceType.Queen;
                            }

                            _chessBoard[move.To.Item1, move.To.Item2] = currentPiece;


                            if (pieceTaken != null)
                            {
                                if (pieceTaken.Id == PieceIdentifier.K)
                                {
                                    LogMessage("The {0} king has died, expecting the !!! button to be pressed.",
                                        _blackPlaying ? "White" : "Black");
                                    if (_blackPlaying)
                                    {
                                        _whiteKingDead = true;
                                    }
                                    else
                                    {
                                        _blackKingDead = true;
                                    }
                                }
                                else
                                {
                                    LogMessage("{0} {1} has been killed.", _blackPlaying ? "White" : "Black",
                                        pieceTaken.Id);
                                }
                            }
                            else
                            {
                                LogMessage("That space was empty.");
                            }

                            LogBoard(_chessBoard);

                            if (_blackPlaying)
                            {
                                _blackMoveQueue.Advance();
                            }
                            else
                            {
                                _whiteMoveQueue.Advance();
                            }

                            _blackPlaying = !_blackPlaying;
                            return;
                        }
                    }
                }

                LogMessage("{0} {1} is dead, advancing to the next position in the queue.",
                    _blackPlaying ? "Black" : "White", _blackPlaying ? _blackMoveQueue.CurrentPiece.ToString() : _whiteMoveQueue.CurrentPiece.ToString());

                if (_blackPlaying)
                {
                    _blackMoveQueue.Advance();
                }
                else
                {
                    _whiteMoveQueue.Advance();
                }
            }
        }
    }

    private IEnumerator HoldChecker()
    {
        yield return new WaitForSeconds(1f);
        _holding = true;
        _clearMode = true;
        ScreenText.text = string.Empty;
        ScreenRenderer.material = ScreenMats[2];
    }

    private static Move GetMove(List<Move> possibleMoves, ChessPiece[,] board)
    {
        foreach (var move in possibleMoves)
        {
            if (board[move.To.Item1, move.To.Item2] != null && board[move.To.Item1, move.To.Item2].Id == PieceIdentifier.K)
            {
                return move;
            }
        }

        return possibleMoves.PickRandom();
    }
    

    private void PlacePiece(BoardItem piece, int coorY, int coorX)
    {
        var index = ShoddyChessHelper.GetIndexFromCoordinate(new Pair<int, int>(coorY, coorX));
        Pieces[index].SetActive(true);
        Pieces[index]
                .GetComponentInChildren<TextMesh>().color =
            piece.Color == PieceColor.White ? Color.black : Color.white;
        Pieces[index]
                .GetComponent<MeshRenderer>().material =
            piece.Color == PieceColor.White ? PieceMats[0] : PieceMats[1];
        Pieces[index].SetActive(true);
        Pieces[index]
                .GetComponentInChildren<TextMesh>().text =
            Constants._symbols[(int) piece.Id];
        _inputtedBoard[coorY, coorX] = piece;
        Pieces[index].GetComponentInChildren<TextMesh>().fontSize =
            (piece.Id == PieceIdentifier.Q || piece.Id == PieceIdentifier.K) ? 100 : 70;
    }
    
    private void LogBoard(object boardItems)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        if (boardItems.GetType() == typeof(BoardItem[,]))
        {
            var items = boardItems as BoardItem[,];
            if (items == null)
            {
                throw new InvalidOperationException("Error in LogBoard typeof Boarditem[,]");
            }
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (items[y, x] == null)
                    {
                        sb.Append("###");
                    }
                    else
                    {
                        sb.AppendFormat(items[y, x].ToString());
                        if (items[y, x].Id == PieceIdentifier.Q || items[y, x].Id == PieceIdentifier.K)
                        {
                            sb.Append("#");
                        }
                    }
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
        }
        else
        {
            var items = boardItems as ChessPiece[,];

            if (items == null)
            {
                throw new InvalidOperationException("Error in LogBoard typeof ChessPiece[,]");
            }
            
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (items[y, x] == null)
                    {
                        sb.Append("###");
                    }
                    else
                    {
                        sb.AppendFormat(items[y, x].ToString());
                        if (items[y, x].Id == PieceIdentifier.Q || items[y, x].Id == PieceIdentifier.K)
                        {
                            sb.Append("#");
                        }
                    }

                    sb.Append(" ");
                }
                sb.AppendLine();
            }
        }
        
        LogMessage(sb.ToString());
    }
    
    private IEnumerator ShowStages()
    {
        _isShowingStages = true;
        _isInSolvingPhase = false;
        StartCoroutine(ChangeState(false));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < _stages.Count; i++)
        {
            if (i == 0)
            {
                StageText.text = "----";
            }
            else
            {
                Audio.PlaySoundAtTransform(Sounds[2].name, Module.transform);
                ChangeStageText(i);
            }

            InformationText.text = _stages[i];
            yield return new WaitForSeconds(1.5f);
        }

        _isInSolvingPhase = true;
        StartCoroutine(ChangeState(true));
        _isShowingStages = false;
    }

    private void LogMessage(string message, params object[] parameters)
    {
        Debug.LogFormat("[Shoddy Chess #{0}] {1}", _moduleId, string.Format(message, parameters));
    }

    private void Update()
    {
        ++_buffer;
        if (_buffer == 10)
        {
            _buffer = 0;
            if (!_isSolved)
            {
                if (_stage < Info.GetSolvedModuleNames().Count(x => !_ignored.Contains(x)))
                {
                    ++_stage;
                    GenerateStage();
                }
            }
        }
    }
}