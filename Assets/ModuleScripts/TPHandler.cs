using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShoddyChess;

public partial class ShoddyChessScript
{
#pragma warning disable 414
    private const string TwitchHelpMessage =
        "Place a piece(s) in a specific location using !{0} place wq1 a2 a3 a4 wb1 b2 wp8 e5, etc. Clear the board using !{0} clear all. Or clear a specific coordinate using !{0} clear a4 b2. (Queen = q;Knight = n;Bishop = b;Rook = r;Pawn = p) Example: BK1 would place a black king, WP3 would place a white pawn number 3. Use !{0} button, to press the !!! button. Submit your answer using !{0} submit. NOTE: When placing a queen, you must put a number after the WQ or BQ, for example WQ1 or BQ4.";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();

        if (_isInSolvingPhase)
        {
            if (command.EqualsAny("clear all", "clear board", "clear everything"))
            {
                yield return null;
                if (!_clearMode)
                {
                    PieceSwitcherButtons[2].OnInteract();
                    yield return new WaitForSeconds(1.1f);
                    PieceSwitcherButtons[2].OnInteractEnded();
                    yield return new WaitForSeconds(.1f);
                }

                for (var k = 0; k < 8; k++)
                {
                    for (var l = 0; l < 8; l++)
                    {
                        var index = ShoddyChessHelper.GetIndexFromCoordinate(new Pair<int, int>(k, l));
                        var selectable = Pieces[index].GetComponentInParent<KMSelectable>();

                        if (_inputtedBoard[k, l] != null && _inputtedBoard[k, l].Id != PieceIdentifier.K)
                        {
                            selectable.OnInteract();
                            yield return new WaitForSeconds(.01f);
                        }
                    }
                }

                PieceSwitcherButtons[2].OnInteract();
                yield return new WaitForSeconds(.1f);
                PieceSwitcherButtons[2].OnInteractEnded();
                yield break;
            }

            if (command.EqualsAny("submit", "press submit", "press king", "check", "king"))
            {
                yield return null;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        if (_inputtedBoard[y, x] == null)
                        {
                            continue;
                        }

                        if (_inputtedBoard[y, x].Id == PieceIdentifier.K)
                        {
                            Pieces[ShoddyChessHelper.GetIndexFromCoordinate(new Pair<int, int>(y, x))]
                                .GetComponentInParent<KMSelectable>().OnInteract();
                            yield break;
                        }
                    }
                }
            }


            var clearMatch = Constants.tpClearRegex.Match(command);
            if (clearMatch.Success)
            {
                yield return null;
                var parts = clearMatch.Groups[1].ToString().Split(' ');

                var selectables = new List<KMSelectable>();

                foreach (var part in parts)
                {
                    var col = char.ToUpper(part[0]) - 65;
                    var row =  8 - int.Parse(part[1].ToString());
                    selectables.Add(Pieces[ShoddyChessHelper.GetIndexFromCoordinate(new Pair<int, int>(row, col))]
                        .GetComponentInParent<KMSelectable>());
                }

                if (!_clearMode)
                {
                    PieceSwitcherButtons[2].OnInteract();
                    yield return new WaitForSeconds(1.1f);
                    PieceSwitcherButtons[2].OnInteractEnded();
                    yield return new WaitForSeconds(.1f);
                }

                yield return selectables;

                yield return new WaitForSeconds(.1f);
                PieceSwitcherButtons[2].OnInteract();
                yield return new WaitForSeconds(.1f);
                PieceSwitcherButtons[2].OnInteractEnded();
                yield break;
            }

            var placeMatch = Constants.tpPlaceRegex.Match(command);

            if (placeMatch.Success)
            {
                var groups = placeMatch.Groups[1].ToString().Split(' ');
                //Check so that the command is valid:
                foreach (var part in groups)
                {
                    if (part.Length == 2)
                    {
                        continue;
                    }

                    var piece = ShoddyChessHelper.GetPiece(part);

                    if (piece == null)
                    {
                        yield return string.Format(
                            "sendtochaterror Do you honestly expect me to know what {0} means? 4Head",
                            part.ToUpperInvariant());
                        yield break;
                    }
                }

                yield return null;
                foreach (var part in groups)
                {
                    if (part.Length == 2)
                    {
                        var col = char.ToUpper(part[0]) - 65;
                        var row = 8 - int.Parse(part[1].ToString());
                        Pieces[ShoddyChessHelper.GetIndexFromCoordinate(new Pair<int, int>(row, col))]
                            .GetComponentInParent<KMSelectable>().OnInteract();
                        yield return new WaitForSeconds(.01f);
                        continue;
                    }

                    var piece = ShoddyChessHelper.GetPiece(part);
                    if (CurrentItem.Color != piece.Color)
                    {
                        PieceSwitcherButtons[2].OnInteract();
                        yield return new WaitForSeconds(.1f);
                        PieceSwitcherButtons[2].OnInteractEnded();
                        yield return new WaitForSeconds(.1f);
                    }

                    while (CurrentItem.Id != piece.Id)
                    {
                        PieceSwitcherButtons[1].OnInteract();
                        yield return new WaitForSeconds(.01f);
                    } 
                }
            }
        }
        else
        {
            if (command.EqualsAny("button", "!!!", "press button", "press !!!"))
            {
                yield return null;
                InformationButton.OnInteract();
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        _isSolved = true;
        Module.HandlePass();
        LogMessage("Force solve requested by twitch plays.");
        yield break;
    }
}