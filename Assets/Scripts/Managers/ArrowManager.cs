using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{

    public float AnimationSpeed = 2f;

    public Goal TheGoal;

    public bool ShowArrows;
    public Arrow MyArrow;

    public Sprite[] StraightArrows;
    public Sprite[] TurnArrows;
    public Sprite[] VerticalMergeArrows;
    public Sprite[] HorizontalMergeArrows;
    public Sprite[] ThreeWayMergeArrows;

    private Dictionary<Pathing, List<Vector2>> paths;
    private Dictionary<Vector2, Arrow> arrows;

    protected void Awake()
    {
        if (MyArrow == null)
            Debug.LogError(name + ": No arrow set!");

        if (TheGoal == null)
            Debug.LogError(name + ": No goal set!");

        paths = new Dictionary<Pathing, List<Vector2>>();
        arrows = new Dictionary<Vector2, Arrow>();
    }

    protected void Update()
    {

    }

    // Checks for unique sources
    public void AddPath(Pathing source)
    {
        if (paths.ContainsKey(source))
            RemovePath(source);

        paths.Add(source, source.Path);

        // Create the arrows
        for (int i = 0; i < source.Path.Count - 1; i++)
        {
            Vector2 newDir = (source.Path[i + 1] - source.Path[i]).normalized;
            Vector2 sourceArrow = (i - 1 < 0) ? (Vector2)source.transform.position : source.Path[i - 1];

            // Check if we already have an arrow in place, and if so merge
            if (arrows.ContainsKey(source.Path[i]))
            {
                Arrow arrow = arrows[source.Path[i]];
                Merge(sourceArrow, newDir, arrow);
                return;
            } else // Otherwise create a new arrow
            {
                Arrow arrow = Instantiate(MyArrow, source.Path[i], Quaternion.identity, transform);
                arrows.Add(source.Path[i], arrow);
                arrow.ArrowDirection = newDir;
                arrow.gameObject.SetActive(ShowArrows);
                Vector2 sourceDir = ((Vector2)arrow.transform.position - sourceArrow).normalized;

                // Check whether this is a straight or curved arrow
                if (sourceDir == newDir)
                {
                    arrow.ArrowType = Arrow.Type.Straight;
                    arrow.Flipped = false;
                    arrow.UpdateGraphics(StraightArrows);
                } else
                {
                    // FIXME: Fix bug with curved arrow displays
                    arrow.ArrowType = Arrow.Type.Curved;
                    arrow.Flipped = sourceDir != Vector2.Perpendicular(newDir);
                    arrow.UpdateGraphics(TurnArrows);
                }
            }
        }
    }

    private void Merge(Vector2 sourceArrow, Vector2 newDir, Arrow arrow)
    {
        Vector2 sourceDir = ((Vector2)arrow.transform.position - sourceArrow).normalized;
        switch (arrow.ArrowType)
        {
            // Straight arrows
            case Arrow.Type.Straight:
                if (newDir == arrow.ArrowDirection && sourceDir != newDir)
                {
                    arrow.ArrowType = Arrow.Type.HorizontalMerge;
                    arrow.Flipped = Vector2.Perpendicular(arrow.ArrowDirection) != sourceDir;
                    arrow.UpdateGraphics(HorizontalMergeArrows);
                }
                break;
            // Curved arrows
            case Arrow.Type.Curved:
                if (newDir == arrow.ArrowDirection)
                {
                    arrow.ArrowType = Arrow.Type.HorizontalMerge;
                    arrow.UpdateGraphics(HorizontalMergeArrows);
                } else if (Mathf.Abs(newDir.x) == Mathf.Abs(arrow.ArrowDirection.x) &&
                           Mathf.Abs(newDir.y) == Mathf.Abs(arrow.ArrowDirection.y))
                {
                    arrow.ArrowType = Arrow.Type.VerticalMerge;
                    arrow.UpdateGraphics(VerticalMergeArrows);
                }
                break;
            // Arrows that already have a merge
            case Arrow.Type.HorizontalMerge:
                if (newDir != arrow.ArrowDirection)
                {
                    arrow.ArrowType = Arrow.Type.ThreeWayMerge;
                    arrow.UpdateGraphics(ThreeWayMergeArrows);
                }
                break;
            case Arrow.Type.VerticalMerge:
                if (newDir == arrow.ArrowDirection)
                {
                    arrow.ArrowType = Arrow.Type.ThreeWayMerge;
                    arrow.UpdateGraphics(ThreeWayMergeArrows);
                }
                break;
            // Should not get here?
            default:
                Debug.LogWarning(name + ": End of case statement reached!");
                break;
        }
    }

    private void RemovePath(Pathing source)
    {
        List<Vector2> path = paths[source];

        // Remove arrows
        for (int i = 0; i < path.Count - 1; i++)
            RemoveArrow(path[i], path[i + 1]);

        // Remove path
        paths.Remove(source);
    }

    // Removes arrow, as long as it is not part of another path
    private void RemoveArrow(Vector2 target, Vector2 next)
    {
        Vector2 direction = (next - target).normalized;

        // If this arrow is in use by another path, do not remove it
        if (Vector2.up != direction && arrows.ContainsKey(target + Vector2.up))
            return;
        if (Vector2.down != direction && arrows.ContainsKey(target + Vector2.down))
            return;
        if (Vector2.left != direction && arrows.ContainsKey(target + Vector2.left))
            return;
        if (Vector2.right != direction && arrows.ContainsKey(target + Vector2.right))
            return;

        arrows.Remove(target);
    }

}
