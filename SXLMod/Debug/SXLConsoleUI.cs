using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace SXLMod.Debug
{
    public static class SXLConsoleUI
    {
        public static void CreateDeveloperConsole()
        {
            GameObject go = new GameObject("SXLDeveloperConsole");

            // Build all children components of console window before assinging the developer component

            // Build Canvas
            GameObject canvas = SXLConsoleUI.ConsoleCanvas();
            // Build ScrollView

            // Build InputField

            // Build Panel
            GameObject panel = SXLConsoleUI.ConsolePanel();

            // Add component and set up console variables
            SXLDeveloperConsole devConsole = go.AddComponent<SXLDeveloperConsole>();

        }

        private static GameObject ConsoleCanvas()
        {
            GameObject go = new GameObject("Canvas");
            go.AddComponent<RectTransform>();
            go.AddComponent<Canvas>();  // Screen Space Overlay | Sort Order 0
            go.AddComponent<CanvasScaler>();  // Constant Pixel Size | Scale Factor=1 | Reference Pixels Per Unity=100
            go.AddComponent<GraphicRaycaster>();  // Ignore Reversed Graphics=true | Blocking Objects=None | Blocking Mask=Everything

            return go;
        }

        private static GameObject ConsolePanel()
        {
            GameObject go = new GameObject("Panel");
            // Set Rect Transform
            RectTransform xform = go.AddComponent<RectTransform>();
            xform.offsetMax = new Vector2(50f, 50f);
            xform.offsetMin = new Vector2(50f, 50f);
            xform.anchorMin = new Vector2(0f, 0f);
            xform.anchorMax = new Vector2(1f, 1f);
            xform.pivot = new Vector2(0.5f, 0.5f);

            go.AddComponent<CanvasRenderer>();
            Image image = go.AddComponent<Image>();
            image.color = new Color(255f, 255f, 255f, 127f);

            return go;
        }

        public static GameObject ConsoleScrollView()
        {
            GameObject go = new GameObject("Scroll View");
            GameObject viewport = new GameObject("Viewport");
            GameObject content = new GameObject("Content");
            GameObject text = new GameObject("Text");

            //Scroll View
            go.AddComponent<CanvasRenderer>();
            RectTransform scrollViewxform = go.AddComponent<RectTransform>();
            scrollViewxform.offsetMin = new Vector2(50f, 100f);
            scrollViewxform.offsetMax = new Vector2(50f, 50f);
            scrollViewxform.anchorMin = new Vector2(0f, 0f);
            scrollViewxform.anchorMax = new Vector2(1f, 1f);
            scrollViewxform.pivot = new Vector2(0.5f, 0.5f);
            ScrollRect scrollRect = go.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.inertia = true;
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            Image scrollImage = go.AddComponent<Image>();
            scrollImage.color = new Color(0f, 0f, 0f, 180f);

            // Viewport
            viewport.AddComponent<CanvasRenderer>();
            viewport.AddComponent<Mask>();
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(255f, 255f, 255f, 255f);
            RectTransform viewportXform = viewport.AddComponent<RectTransform>();
            viewportXform.offsetMin = new Vector2(0f, 0f);
            viewportXform.offsetMax = new Vector2(20f, 0);
            viewportXform.anchorMin = new Vector2(0f, 0f);
            viewportXform.anchorMax = new Vector2(1f, 1f);
            viewportXform.pivot = new Vector2(0.0f, 1.0f);

            // Content
            VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding.left = 10;
            contentLayout.padding.right = 10;
            contentLayout.padding.top = 10;
            contentLayout.padding.bottom = 0;
            contentLayout.childAlignment = TextAnchor.LowerLeft;
            contentLayout.childControlHeight = true;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandHeight = false;
            contentLayout.childForceExpandWidth = false;
            ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform contentXform = content.AddComponent<RectTransform>();
            contentXform.anchorMin = new Vector2(0f, 1f);
            contentXform.anchorMax = new Vector2(1f, 1f);
            contentXform.pivot = new Vector2(0.0f, 1.0f);

            // Text
            text.AddComponent<CanvasRenderer>();
            Text t = text.AddComponent<Text>();
            t.alignment = TextAnchor.UpperLeft;
            t.color = new Color(255f, 255f, 255f, 255f);
            RectTransform textXform = text.AddComponent<RectTransform>();
            textXform.pivot = new Vector2(0.5f, 0.5f);

            // Composition
            viewport.transform.parent = go.transform;

        }
    }
}
