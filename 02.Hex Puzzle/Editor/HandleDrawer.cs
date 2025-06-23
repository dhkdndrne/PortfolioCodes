using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

static class HandleDrawer
{
    static bool isEnable = false;
    private static Hex hex;
    static int m_mode = 0;

    public static void SetEnable(bool enable)
    {
        if (!isEnable && enable)
        {
            SceneView.duringSceneGui += Draw;
            isEnable = true;
        }
        else if (isEnable && !enable)
        {
            SceneView.duringSceneGui -= Draw;
            isEnable = false;
        }
    }

    public static void SetHandleIndex(Hex h, int mode)
    {
        hex = h;
        m_mode = mode;
    }

    public static void Draw(SceneView view)
    {
        var board = Board_Edit.Instance;

        // Draw Handle Gizmo
        if (m_mode == 0)
        {
            Handles.color = Color.green;
        }
        else if (m_mode == 1)
        {
            Handles.color = Color.red;
        }
        else if (m_mode == 2)
        {
            Handles.color = Color.yellow;
        }
        else if (m_mode == 3)
        {
            Handles.color = Color.blue;
        }

        var handlePos = board.IndexToWorldPos(hex);
        Vector3 size = new Vector3(board.ColGap, board.RowGap, 1);
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        Handles.DrawWireCube(handlePos, size);

        // (must repaint to update draws)
        view.Repaint();
    }
}

