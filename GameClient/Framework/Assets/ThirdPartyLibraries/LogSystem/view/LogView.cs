using System.Collections.Generic;
using UnityEngine;

namespace LogSystem
{
    public class LogView : ILoggerInterface
    {
        private LogGUI _logGui; //画 UI 时使用到

        private LogCacheData CacheData;

        public void Initialize()
        {
            CacheData = LogManager.GetLogCacheData();
            _logGui = new LogGUI();
            _logGui.LoadImages();
            _logGui.LoadGUIContent();
            _logGui.LoadStyle();
        }

        public void Update()
        {
            calculateStartIndex();
            List<LogEntity> currentLog = CacheData.currentLog;
            Vector2 size = CacheData.size;
            int totalCount = currentLog.Count;
            int totalVisibleCount = (int)(Screen.height * 0.75f / size.y);
            if (startIndex >= (totalCount - totalVisibleCount))
                scrollPosition.y += size.y;
        }

        public void UnInitialize()
        {
            _logGui = null;
        }

        //展示的Log界面类型
        enum ReportView
        {
            None,
            Logs,
            Info,
            Snapshot,
        }

        ReportView currentView = ReportView.Logs;


        Rect screenRect = Rect.zero;
        Rect toolBarRect = Rect.zero;
        Rect logsRect = Rect.zero;
        Rect stackRect = Rect.zero;
        Rect graphRect = Rect.zero;
        Rect graphMinRect = Rect.zero;
        Rect graphMaxRect = Rect.zero;
        Rect buttomRect = Rect.zero;
        Vector2 stackRectTopLeft;
        Rect detailRect = Rect.zero;

        Vector2 scrollPosition;
        Vector2 scrollPosition2;
        Vector2 toolbarScrollPosition;


        float toolbarOldDrag = 0;
        float oldDrag;
        float oldDrag2;
        float oldDrag3;
        int startIndex;

        void calculateStartIndex()
        {
            List<LogEntity> currentLog = CacheData.currentLog;
            Vector2 size = CacheData.size;

            startIndex = (int)(scrollPosition.y / size.y);
            startIndex = Mathf.Clamp(startIndex, 0, currentLog.Count);
        }

        //calculate what is the currentLog : collapsed or not , hide or view warnings ......
        void calculateCurrentLog()
        {
            List<LogEntity> currentLog = CacheData.currentLog;
            string filterText = CacheData.filterText;
            List<LogEntity> collapsedLogs = CacheData.collapsedLogs;
            bool collapse = CacheData.collapse;
            bool showLog = CacheData.showLog;
            bool showWarning = CacheData.showWarning;
            bool showError = CacheData.showError;
            LogMultiKeyDictionary<string, string, LogEntity> logsDic = CacheData.logsDic;
            List<LogEntity> logs = CacheData.logs;
            Vector2 size = CacheData.size;

            bool filter = !string.IsNullOrEmpty(filterText);
            string _filterText = "";
            if (filter)
                _filterText = filterText.ToLower();
            currentLog.Clear();
            if (collapse)
            {
                for (int i = 0; i < collapsedLogs.Count; i++)
                {
                    LogEntity log = collapsedLogs[i];
                    if (log.logType == LogType.Log && !showLog)
                        continue;
                    if (log.logType == LogType.Warning && !showWarning)
                        continue;
                    if (log.logType == LogType.Error && !showError)
                        continue;
                    if (log.logType == LogType.Assert && !showError)
                        continue;
                    if (log.logType == LogType.Exception && !showError)
                        continue;

                    if (filter)
                    {
                        if (log.condition.ToLower().Contains(_filterText))
                            currentLog.Add(log);
                    }
                    else
                    {
                        currentLog.Add(log);
                    }
                }
            }
            else
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    LogEntity log = logs[i];
                    if (log.logType == LogType.Log && !showLog)
                        continue;
                    if (log.logType == LogType.Warning && !showWarning)
                        continue;
                    if (log.logType == LogType.Error && !showError)
                        continue;
                    if (log.logType == LogType.Assert && !showError)
                        continue;
                    if (log.logType == LogType.Exception && !showError)
                        continue;

                    if (filter)
                    {
                        if (log.condition.ToLower().Contains(_filterText))
                            currentLog.Add(log);
                    }
                    else
                    {
                        currentLog.Add(log);
                    }
                }
            }

            if (CacheData.selectedLog != null)
            {
                int newSelectedIndex = currentLog.IndexOf(CacheData.selectedLog);
                if (newSelectedIndex == -1)
                {
                    LogEntity collapsedSelected = logsDic[CacheData.selectedLog.condition][CacheData.selectedLog.stacktrace];
                    newSelectedIndex = currentLog.IndexOf(collapsedSelected);
                    if (newSelectedIndex != -1)
                        scrollPosition.y = newSelectedIndex * size.y;
                }
                else
                {
                    scrollPosition.y = newSelectedIndex * size.y;
                }
            }
        }

        Rect countRect = Rect.zero;
        Rect timeRect = Rect.zero;
        Rect timeLabelRect = Rect.zero;
        Rect sceneRect = Rect.zero;
        Rect sceneLabelRect = Rect.zero;
        Rect memoryRect = Rect.zero;
        Rect memoryLabelRect = Rect.zero;
        Rect fpsRect = Rect.zero;
        Rect fpsLabelRect = Rect.zero;
        GUIContent tempContent = new GUIContent();
        Rect tempRect;


        public void DrawGUI()
        {
            Vector2 size = CacheData.size;

            screenRect.x = 0;
            screenRect.y = 0;
            screenRect.width = Screen.width;
            screenRect.height = Screen.height;

            getDownPos();


            logsRect.x = 0f;
            logsRect.y = size.y * 2f;
            logsRect.width = Screen.width;
            logsRect.height = Screen.height * 0.75f - size.y * 2f;

            stackRectTopLeft.x = 0f;
            stackRect.x = 0f;
            stackRectTopLeft.y = Screen.height * 0.75f;
            stackRect.y = Screen.height * 0.75f;
            stackRect.width = Screen.width;
            stackRect.height = Screen.height * 0.25f - size.y;



            detailRect.x = 0f;
            detailRect.y = Screen.height - size.y * 3;
            detailRect.width = Screen.width;
            detailRect.height = size.y * 3;

            if (currentView == ReportView.Info)
            {
                DrawInfo();
            }
            else if (currentView == ReportView.Logs)
            {
                DrawToolBar();
                DrawLogs();
            }
        }


        void DrawToolBar()
        {
            Vector2 size = CacheData.size;

            toolBarRect.x = 0f;
            toolBarRect.y = 0f;
            toolBarRect.width = Screen.width;
            toolBarRect.height = size.y * 2f;

            //toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
            //toolbarScrollerSkin.horizontalScrollbar.fixedHeight= 0f  ;

            GUI.skin = _logGui.toolbarScrollerSkin;
            Vector2 drag = getDrag();
            if ((drag.x != 0) && (downPos != Vector2.zero) && (downPos.y > Screen.height - size.y * 2f))
            {
                toolbarScrollPosition.x -= (drag.x - toolbarOldDrag);
            }

            toolbarOldDrag = drag.x;
            GUILayout.BeginArea(toolBarRect);
            toolbarScrollPosition = GUILayout.BeginScrollView(toolbarScrollPosition);
            GUILayout.BeginHorizontal(_logGui.barStyle);

            if (GUILayout.Button(_logGui.clearContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.clear();
            }

            if (GUILayout.Button(_logGui.collapseContent,
                                 (CacheData.collapse)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.collapse = !CacheData.collapse;
                calculateCurrentLog();
            }

            if (CacheData.showClearOnNewSceneLoadedButton &&
                GUILayout.Button(_logGui.clearOnNewSceneContent,
                                 (CacheData.clearOnNewSceneLoaded)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle,
                                 GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.clearOnNewSceneLoaded =
                    !CacheData.clearOnNewSceneLoaded;
            }

            if (CacheData.showTimeButton &&
                GUILayout.Button(_logGui.showTimeContent,
                                 (CacheData.showTime)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle,
                                 GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showTime = !CacheData.showTime;
            }

            if (CacheData.showSceneButton)
            {
                tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), _logGui.lowerLeftFontStyle);
                if (GUILayout.Button(_logGui.showSceneContent,
                                     (CacheData.showScene)
                                         ? _logGui.buttonActiveStyle
                                         : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
                {
                    CacheData.showScene = !CacheData.showScene;
                }

                tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(tempRect, CacheData.currentScene, _logGui.lowerLeftFontStyle);
            }

            if (CacheData.showMemButton)
            {
                if (GUILayout.Button(_logGui.showMemoryContent,
                                     (CacheData.showMemory)
                                         ? _logGui.buttonActiveStyle
                                         : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
                {
                    CacheData.showMemory = !CacheData.showMemory;
                }

                tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(tempRect, CacheData.GCTotalMemory.ToString("0.0"),
                          _logGui.lowerLeftFontStyle);
            }

            if (CacheData.showFpsButton)
            {
                if (GUILayout.Button(_logGui.showFpsContent,
                                     (CacheData.showFps)
                                         ? _logGui.buttonActiveStyle
                                         : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
                {
                    CacheData.showFps = !CacheData.showFps;
                }

                tempRect = GUILayoutUtility.GetLastRect();
                GUI.Label(tempRect, LogManager.GetLogFps().fpsText, _logGui.lowerLeftFontStyle);
            }
            if (GUILayout.Button(_logGui.graphContent, (CacheData.showGraph) ? _logGui.buttonActiveStyle : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showGraph = !CacheData.showGraph;
            }
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, CacheData.samples.Count.ToString(), _logGui.lowerLeftFontStyle);

            if (CacheData.showSearchText)
            {
                GUILayout.Box(_logGui.searchContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                              GUILayout.Height(size.y * 2));
                tempRect = GUILayoutUtility.GetLastRect();
                string newFilterText =
                    GUI.TextField(tempRect, CacheData.filterText, _logGui.searchStyle);
                if (newFilterText != CacheData.filterText)
                {
                    CacheData.filterText = newFilterText;
                    calculateCurrentLog();
                }
            }

            if (CacheData.showCopyButton)
            {
                if (GUILayout.Button(_logGui.copyContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                     GUILayout.Height(size.y * 2)))
                {
                    if (CacheData.selectedLog == null)
                        GUIUtility.systemCopyBuffer = "No log selected";
                    else
                        GUIUtility.systemCopyBuffer = CacheData.selectedLog.condition + System.Environment.NewLine +
                                                      System.Environment.NewLine + CacheData.selectedLog.stacktrace;
                }
            }

            if (CacheData.showSaveButton)
            {
                if (GUILayout.Button(_logGui.saveLogsContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                     GUILayout.Height(size.y * 2)))
                {
                    LogManager.GetLogFile().UploadLog();
                }
            }

            if (GUILayout.Button(_logGui.infoContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                currentView = ReportView.Info;
            }



            GUILayout.FlexibleSpace();


            string logsText = " ";
            if (CacheData.collapse)
            {
                logsText += CacheData.numOfCollapsedLogs;
            }
            else
            {
                logsText += CacheData.numOfLogs;
            }

            string logsWarningText = " ";
            if (CacheData.collapse)
            {
                logsWarningText += CacheData.numOfCollapsedLogsWarning;
            }
            else
            {
                logsWarningText += CacheData.numOfLogsWarning;
            }

            string logsErrorText = " ";
            if (CacheData.collapse)
            {
                logsErrorText += CacheData.numOfCollapsedLogsError;
            }
            else
            {
                logsErrorText += CacheData.numOfLogsError;
            }

            GUILayout.BeginHorizontal((CacheData.showLog)
                                          ? _logGui.buttonActiveStyle
                                          : _logGui.barStyle);
            if (GUILayout.Button(_logGui.logContent, _logGui.nonStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.showLog = !CacheData.showLog;
                calculateCurrentLog();
            }

            if (GUILayout.Button(logsText, _logGui.nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showLog = !CacheData.showLog;
                calculateCurrentLog();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal((CacheData.showWarning)
                                          ? _logGui.buttonActiveStyle
                                          : _logGui.barStyle);
            if (GUILayout.Button(_logGui.warningContent, _logGui.nonStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.showWarning = !CacheData.showWarning;
                calculateCurrentLog();
            }

            if (GUILayout.Button(logsWarningText, _logGui.nonStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.showWarning = !CacheData.showWarning;
                calculateCurrentLog();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal((CacheData.showError)
                                          ? _logGui.buttonActiveStyle
                                          : _logGui.nonStyle);
            if (GUILayout.Button(_logGui.errorContent, _logGui.nonStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.showError = !CacheData.showError;
                calculateCurrentLog();
            }

            if (GUILayout.Button(logsErrorText, _logGui.nonStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.showError = !CacheData.showError;
                calculateCurrentLog();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button(_logGui.closeContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                CacheData.IsShowGUI = false;
                LogManager.Instance.DestroyDrawGUI();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        void DrawLogs()
        {
            Vector2 size = CacheData.size;
            List<LogEntity> currentLog = CacheData.currentLog;
            GUILayout.BeginArea(logsRect, _logGui.backStyle);

            GUI.skin = _logGui.logScrollerSkin;
            //setStartPos();
            Vector2 drag = getDrag();

            if (drag.y != 0 && logsRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
            {
                scrollPosition.y += (drag.y - oldDrag);
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            oldDrag = drag.y;


            int totalVisibleCount = (int)(Screen.height * 0.75f / size.y);
            int totalCount = currentLog.Count;
            /*if( totalCount < 100 )
                inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 0;
            else 
                inGameLogsScrollerSkin.verticalScrollbarThumb.fixedHeight = 64;*/

            totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - startIndex);
            int index = 0;
            int beforeHeight = (int)(startIndex * size.y);
            //selectedIndex = Mathf.Clamp( selectedIndex , -1 , totalCount -1);
            if (beforeHeight > 0)
            {
                //fill invisible gap before scroller to make proper scroller pos
                GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
                GUILayout.Label("---");
                GUILayout.EndHorizontal();
            }

            int endIndex = startIndex + totalVisibleCount;
            endIndex = Mathf.Clamp(endIndex, 0, totalCount);
            bool scrollerVisible = (totalVisibleCount < totalCount);
            for (int i = startIndex; (startIndex + index) < endIndex; i++)
            {

                if (i >= currentLog.Count)
                    break;
                LogEntity log = currentLog[i];

                if (log.logType == LogType.Log && !CacheData.showLog)
                    continue;
                if (log.logType == LogType.Warning && !CacheData.showWarning)
                    continue;
                if (log.logType == LogType.Error && !CacheData.showError)
                    continue;
                if (log.logType == LogType.Assert && !CacheData.showError)
                    continue;
                if (log.logType == LogType.Exception && !CacheData.showError)
                    continue;

                if (index >= totalVisibleCount)
                {
                    break;
                }

                GUIContent content = null;
                if (log.logType == LogType.Log)
                    content = _logGui.logContent;
                else if (log.logType == LogType.Warning)
                    content = _logGui.warningContent;
                else
                    content = _logGui.errorContent;
                //content.text = log.condition ;

                GUIStyle currentLogStyle = ((startIndex + index) % 2 == 0) ? _logGui.evenLogStyle : _logGui.oddLogStyle;
                if (log == CacheData.selectedLog)
                {
                    //selectedLog = log ;
                    currentLogStyle = _logGui.selectedLogStyle;
                }
                else
                {
                }

                tempContent.text = log.count.ToString();
                float w = 0f;
                if (CacheData.collapse)
                    w = _logGui.barStyle.CalcSize(tempContent).x + 3;
                countRect.x = Screen.width - w;
                countRect.y = size.y * i;
                if (beforeHeight > 0)
                    countRect.y += 8;//i will check later why
                countRect.width = w;
                countRect.height = size.y;

                if (scrollerVisible)
                    countRect.x -= size.x * 2;

                Sample sample = CacheData.samples[log.sampleId];
                fpsRect = countRect;
                if (CacheData.showFps)
                {
                    tempContent.text = sample.fpsText;
                    w = currentLogStyle.CalcSize(tempContent).x + size.x;
                    fpsRect.x -= w;
                    fpsRect.width = size.x;
                    fpsLabelRect = fpsRect;
                    fpsLabelRect.x += size.x;
                    fpsLabelRect.width = w - size.x;
                }


                memoryRect = fpsRect;
                if (CacheData.showMemory)
                {
                    tempContent.text = sample.memory.ToString("0.000");
                    w = currentLogStyle.CalcSize(tempContent).x + size.x;
                    memoryRect.x -= w;
                    memoryRect.width = size.x;
                    memoryLabelRect = memoryRect;
                    memoryLabelRect.x += size.x;
                    memoryLabelRect.width = w - size.x;
                }
                sceneRect = memoryRect;
                if (CacheData.showScene)
                {

                    tempContent.text = sample.sceneName;
                    w = currentLogStyle.CalcSize(tempContent).x + size.x;
                    sceneRect.x -= w;
                    sceneRect.width = size.x;
                    sceneLabelRect = sceneRect;
                    sceneLabelRect.x += size.x;
                    sceneLabelRect.width = w - size.x;
                }
                timeRect = sceneRect;
                if (CacheData.showTime)
                {
                    tempContent.text = sample.time.ToString("0.000");
                    w = currentLogStyle.CalcSize(tempContent).x + size.x;
                    timeRect.x -= w;
                    timeRect.width = size.x;
                    timeLabelRect = timeRect;
                    timeLabelRect.x += size.x;
                    timeLabelRect.width = w - size.x;
                }



                GUILayout.BeginHorizontal(currentLogStyle);
                if (log == CacheData.selectedLog)
                {
                    GUILayout.Box(content, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                    GUILayout.Label(log.condition, _logGui.selectedLogFontStyle);
                    //GUILayout.FlexibleSpace();
                    if (CacheData.showTime)
                    {
                        GUI.Box(timeRect, _logGui.showTimeContent, currentLogStyle);
                        GUI.Label(timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                    }
                    if (CacheData.showScene)
                    {
                        GUI.Box(sceneRect, _logGui.showSceneContent, currentLogStyle);
                        GUI.Label(sceneLabelRect, sample.sceneName, currentLogStyle);
                    }
                    if (CacheData.showMemory)
                    {
                        GUI.Box(memoryRect, _logGui.showMemoryContent, currentLogStyle);
                        GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                    }
                    if (CacheData.showFps)
                    {
                        GUI.Box(fpsRect, _logGui.showFpsContent, currentLogStyle);
                        GUI.Label(fpsLabelRect, sample.fpsText, currentLogStyle);
                    }


                }
                else
                {
                    if (GUILayout.Button(content, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y)))
                    {
                        //selectedIndex = startIndex + index ;
                        CacheData.selectedLog = log;
                    }
                    if (GUILayout.Button(log.condition, _logGui.logButtonStyle))
                    {
                        //selectedIndex = startIndex + index ;
                        CacheData.selectedLog = log;
                    }
                    //GUILayout.FlexibleSpace();
                    if (CacheData.showTime)
                    {
                        GUI.Box(timeRect, _logGui.showTimeContent, currentLogStyle);
                        GUI.Label(timeLabelRect, sample.time.ToString("0.000"), currentLogStyle);
                    }
                    if (CacheData.showScene)
                    {
                        GUI.Box(sceneRect, _logGui.showSceneContent, currentLogStyle);
                        GUI.Label(sceneLabelRect, sample.sceneName, currentLogStyle);
                    }
                    if (CacheData.showMemory)
                    {
                        GUI.Box(memoryRect, _logGui.showMemoryContent, currentLogStyle);
                        GUI.Label(memoryLabelRect, sample.memory.ToString("0.000") + " mb", currentLogStyle);
                    }
                    if (CacheData.showFps)
                    {
                        GUI.Box(fpsRect, _logGui.showFpsContent, currentLogStyle);
                        GUI.Label(fpsLabelRect, sample.fpsText, currentLogStyle);
                    }
                }
                if (CacheData.collapse)
                    GUI.Label(countRect, log.count.ToString(), _logGui.barStyle);
                GUILayout.EndHorizontal();
                index++;
            }

            int afterHeight = (int)((totalCount - (startIndex + totalVisibleCount)) * size.y);
            if (afterHeight > 0)
            {
                //fill invisible gap after scroller to make proper scroller pos
                GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
                GUILayout.Label(" ");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            buttomRect.x = 0f;
            buttomRect.y = Screen.height - size.y;
            buttomRect.width = Screen.width;
            buttomRect.height = size.y;

            if (CacheData.showGraph)
            {
                DrawGraph();
            }
            else
            {
                DrawStack();
            }
        }

        float graphSize = 4f;
        int startFrame = 0;
        int currentFrame = 0;
        Vector3 tempVector1;
        Vector3 tempVector2;
        Vector2 graphScrollerPos;
        float maxFpsValue;
        float minFpsValue;
        float maxMemoryValue;
        float minMemoryValue;

        void DrawGraph()
        {
            Vector2 size = CacheData.size;

            graphRect = stackRect;
            graphRect.height = Screen.height * 0.25f;//- size.y ;



            //startFrame = samples.Count - (int)(Screen.width / graphSize) ;
            //if( startFrame < 0 ) startFrame = 0 ;
            GUI.skin = _logGui.graphScrollerSkin;

            Vector2 drag = getDrag();
            if (graphRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
            {
                if (drag.x != 0)
                {
                    graphScrollerPos.x -= drag.x - oldDrag3;
                    graphScrollerPos.x = Mathf.Max(0, graphScrollerPos.x);
                }

                Vector2 p = downPos;
                if (p != Vector2.zero)
                {
                    currentFrame = startFrame + (int)(p.x / graphSize);
                }
            }

            oldDrag3 = drag.x;
            GUILayout.BeginArea(graphRect, _logGui.backStyle);

            graphScrollerPos = GUILayout.BeginScrollView(graphScrollerPos);
            startFrame = (int)(graphScrollerPos.x / graphSize);
            if (graphScrollerPos.x >= (CacheData.samples.Count * graphSize - Screen.width))
                graphScrollerPos.x += graphSize;

            GUILayout.Label(" ", GUILayout.Width(CacheData.samples.Count * graphSize));
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            maxFpsValue = 0;
            minFpsValue = 100000;
            maxMemoryValue = 0;
            minMemoryValue = 100000;
            for (int i = 0; i < Screen.width / graphSize; i++)
            {
                int index = startFrame + i;
                if (index >= CacheData.samples.Count)
                    break;
                Sample s = CacheData.samples[index];
                if (maxFpsValue < s.fps) maxFpsValue = s.fps;
                if (minFpsValue > s.fps) minFpsValue = s.fps;
                if (maxMemoryValue < s.memory) maxMemoryValue = s.memory;
                if (minMemoryValue > s.memory) minMemoryValue = s.memory;
            }

            //GUI.BeginGroup(graphRect);


            if (currentFrame != -1 && currentFrame < CacheData.samples.Count)
            {
                Sample selectedSample = CacheData.samples[currentFrame];
                GUILayout.BeginArea(buttomRect, _logGui.backStyle);
                GUILayout.BeginHorizontal();

                GUILayout.Box(_logGui.showTimeContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.time.ToString("0.0"), _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showSceneContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.sceneName, _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showMemoryContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.memory.ToString("0.000"), _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showFpsContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.fpsText, _logGui.nonStyle);
                GUILayout.Space(size.x);

                /*GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
                GUILayout.Label( currentFrame.ToString() ,nonStyle  );*/
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            graphMaxRect = stackRect;
            graphMaxRect.height = size.y;
            GUILayout.BeginArea(graphMaxRect);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_logGui.showMemoryContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(maxMemoryValue.ToString("0.000"), _logGui.nonStyle);

            GUILayout.Box(_logGui.showFpsContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Label(maxFpsValue.ToString("0.000"), _logGui.nonStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            graphMinRect = stackRect;
            graphMinRect.y = stackRect.y + stackRect.height - size.y;
            graphMinRect.height = size.y;
            GUILayout.BeginArea(graphMinRect);
            GUILayout.BeginHorizontal();

            GUILayout.Box(_logGui.showMemoryContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

            GUILayout.Label(minMemoryValue.ToString("0.000"), _logGui.nonStyle);


            GUILayout.Box(_logGui.showFpsContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

            GUILayout.Label(minFpsValue.ToString("0.000"), _logGui.nonStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //GUI.EndGroup();
        }

        void DrawStack()
        {
            Vector2 size = CacheData.size;
            if (CacheData.selectedLog != null)
            {
                Vector2 drag = getDrag();
                if (drag.y != 0 && stackRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
                {
                    scrollPosition2.y += drag.y - oldDrag2;
                }
                oldDrag2 = drag.y;



                GUILayout.BeginArea(stackRect, _logGui.backStyle);
                scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
                Sample selectedSample = null;
                try
                {
                    selectedSample = CacheData.samples[CacheData.selectedLog.sampleId];
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(CacheData.selectedLog.condition, _logGui.stackLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.Space(size.y * 0.25f);
                GUILayout.BeginHorizontal();
                GUILayout.Label(CacheData.selectedLog.stacktrace, _logGui.stackLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.Space(size.y);
                GUILayout.EndScrollView();
                GUILayout.EndArea();


                GUILayout.BeginArea(buttomRect, _logGui.backStyle);
                GUILayout.BeginHorizontal();

                GUILayout.Box(_logGui.showTimeContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.time.ToString("0.000"), _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showSceneContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.sceneName, _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showMemoryContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.memory.ToString("0.000"), _logGui.nonStyle);
                GUILayout.Space(size.x);

                GUILayout.Box(_logGui.showFpsContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
                GUILayout.Label(selectedSample.fpsText, _logGui.nonStyle);
                /*GUILayout.Space( size.x );
                GUILayout.Box( graphContent ,nonStyle, GUILayout.Width(size.x) ,GUILayout.Height(size.y));
                GUILayout.Label( selectedLog.sampleId.ToString() ,nonStyle  );*/
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();



            }
            else
            {
                GUILayout.BeginArea(stackRect, _logGui.backStyle);
                GUILayout.EndArea();
                GUILayout.BeginArea(buttomRect, _logGui.backStyle);
                GUILayout.EndArea();
            }

        }



        Vector2 infoScrollPosition;
        Vector2 oldInfoDrag;
        string buildDate = string.Empty;

        void DrawInfo()
        {
            string deviceModel = CacheData.deviceModel;
            string deviceType = CacheData.deviceType;
            string deviceName = CacheData.deviceName;
            string graphicsMemorySize = CacheData.graphicsMemorySize;
            string maxTextureSize = CacheData.maxTextureSize;
            string systemMemorySize = CacheData.systemMemorySize;
            Vector2 size = CacheData.size;

            GUILayout.BeginArea(screenRect, _logGui.backStyle);

            Vector2 drag = getDrag();
            if ((drag.x != 0) && (downPos != Vector2.zero))
            {
                infoScrollPosition.x -= (drag.x - oldInfoDrag.x);
            }

            if ((drag.y != 0) && (downPos != Vector2.zero))
            {
                infoScrollPosition.y += (drag.y - oldInfoDrag.y);
            }

            oldInfoDrag = drag;

            GUI.skin = _logGui.toolbarScrollerSkin;
            infoScrollPosition = GUILayout.BeginScrollView(infoScrollPosition);
            GUILayout.Space(size.x);
            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.buildFromContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(buildDate, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.systemInfoContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(deviceModel, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(deviceType, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(deviceName, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.graphicsInfoContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(SystemInfo.graphicsDeviceName, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(graphicsMemorySize, _logGui.nonStyle, GUILayout.Height(size.y));

            GUILayout.Space(size.x);
            GUILayout.Label(maxTextureSize, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Space(size.x);
            GUILayout.Space(size.x);
            GUILayout.Label("Screen Width " + Screen.width, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label("Screen Height " + Screen.height, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.showMemoryContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(systemMemorySize + " mb", _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Space(size.x);
            GUILayout.Space(size.x);
            GUILayout.Label("Mem Usage Of Logs " + CacheData.logsMemUsage.ToString("0.000") + " mb",
                            _logGui.nonStyle,
                            GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            //GUILayout.Label( "Mem Usage Of Graph " + graphMemUsage.ToString("0.000")  + " mb", nonStyle , GUILayout.Height(size.y));
            //GUILayout.Space( size.x);
            GUILayout.Label("GC Memory " + CacheData.GCTotalMemory.ToString("0.000") + " mb",
                            _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.softwareContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(SystemInfo.operatingSystem, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.dateContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(System.DateTime.Now.ToString(), _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Label(" - Application Started At " + System.DateTime.Now.ToString(), _logGui.nonStyle,
                            GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.showTimeContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(Time.realtimeSinceStartup.ToString("000"), _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.showFpsContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(LogManager.GetLogFps().fpsText, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.userContent, _logGui.nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.showSceneContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label(CacheData.currentScene, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Box(_logGui.showSceneContent, _logGui.nonStyle, GUILayout.Width(size.x),
                          GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.Label("Unity Version = " + Application.unityVersion, _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            /*GUILayout.BeginHorizontal();
            GUILayout.Space( size.x);
            GUILayout.Box( graphContent ,nonStyle ,  GUILayout.Width(size.x) , GUILayout.Height(size.y));
            GUILayout.Space( size.x);
            GUILayout.Label( "frame " + samples.Count , nonStyle , GUILayout.Height(size.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();*/

            drawInfo_enableDisableToolBarButtons();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Label("Size = " + size.x.ToString("0.0"), _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            float _size = GUILayout.HorizontalSlider(size.x, 16, 64, _logGui.sliderBackStyle, _logGui.sliderThumbStyle,
                                                     GUILayout.Width(Screen.width * 0.5f));
            if (size.x != _size)
            {
                CacheData.size.x = CacheData.size.y = _size;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            if (GUILayout.Button(_logGui.backContent, _logGui.barStyle, GUILayout.Width(size.x * 2),
                                 GUILayout.Height(size.y * 2)))
            {
                currentView = ReportView.Logs;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();



            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }



        void drawInfo_enableDisableToolBarButtons()
        {
            Vector2 size = CacheData.size;

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);
            GUILayout.Label("Hide or Show tool bar buttons", _logGui.nonStyle, GUILayout.Height(size.y));
            GUILayout.Space(size.x);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(size.x);

            if (GUILayout.Button(_logGui.clearOnNewSceneContent,
                                 (CacheData.showClearOnNewSceneLoadedButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle,
                                 GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showClearOnNewSceneLoadedButton =
                    !CacheData.showClearOnNewSceneLoadedButton;
            }

            if (GUILayout.Button(_logGui.showTimeContent,
                                 (CacheData.showTimeButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showTimeButton = !CacheData.showTimeButton;
            }

            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, Time.realtimeSinceStartup.ToString("0.0"), _logGui.lowerLeftFontStyle);
            if (GUILayout.Button(_logGui.showSceneContent,
                                 (CacheData.showSceneButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showSceneButton = !CacheData.showSceneButton;
            }

            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, CacheData.currentScene, _logGui.lowerLeftFontStyle);
            if (GUILayout.Button(_logGui.showMemoryContent,
                                 (CacheData.showMemButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showMemButton = !CacheData.showMemButton;
            }

            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, CacheData.GCTotalMemory.ToString("0.0"),
                      _logGui.lowerLeftFontStyle);

            if (GUILayout.Button(_logGui.showFpsContent,
                                 (CacheData.showFpsButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showFpsButton = !CacheData.showFpsButton;
            }

            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, LogManager.GetLogFps().fpsText, _logGui.lowerLeftFontStyle);
            if (GUILayout.Button(_logGui.graphContent, (CacheData.showGraph) ? _logGui.buttonActiveStyle : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showGraph = !CacheData.showGraph;
            }
            tempRect = GUILayoutUtility.GetLastRect();
            GUI.Label(tempRect, CacheData.samples.Count.ToString(), _logGui.lowerLeftFontStyle);
            if (GUILayout.Button(_logGui.searchContent,
                                 (CacheData.showSearchText)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle,
                                 GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showSearchText = !CacheData.showSearchText;
            }

            if (GUILayout.Button(_logGui.copyContent,
                                 (CacheData.showCopyButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showCopyButton = !CacheData.showCopyButton;
            }

            if (GUILayout.Button(_logGui.saveLogsContent,
                                 (CacheData.showSaveButton)
                                     ? _logGui.buttonActiveStyle
                                     : _logGui.barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
            {
                CacheData.showSaveButton = !CacheData.showSaveButton;
            }

            tempRect = GUILayoutUtility.GetLastRect();
            GUI.TextField(tempRect, CacheData.filterText, _logGui.searchStyle);


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        //calculate drag amount , this is used for scrolling
        Vector2 mousePosition;

        Vector2 getDrag()
        {

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length != 1)
                {
                    return Vector2.zero;
                }

                return Input.touches[0].position - downPos;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    mousePosition = Input.mousePosition;
                    return mousePosition - downPos;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }


        Vector2 downPos;

        Vector2 getDownPos()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
                {
                    downPos = Input.touches[0].position;
                    return downPos;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    downPos.x = Input.mousePosition.x;
                    downPos.y = Input.mousePosition.y;
                    return downPos;
                }
            }

            return Vector2.zero;
        }



    }
}
