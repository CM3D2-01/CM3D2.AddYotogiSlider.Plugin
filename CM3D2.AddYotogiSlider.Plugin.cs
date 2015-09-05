using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityInjector.Attributes;

namespace CM3D2.AddYotogiSlider.Plugin
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginName("CM3D2 AddYotogiSlider"),
    PluginVersion("0.0.0.0")]
    public class AddYotogiSlider : UnityInjector.PluginBase
    {
        public const string Version = "0.0.0.0";

        private int sceneLevel;
        private bool visible = false;
        private PixelValues pv;

        private Maid maid;
        private Rect winRect;
        private Vector2 lastScreenSize;

        private float yotogiSliderWidth = 0.20f;
        private float yotogiSliderHeight = 0.75f;
        private string[] yotogiSliderLabel = {"ãªï±", "ê∏ê_", "óùê´"};
        private bool[] bToggleEnabled =  { false, false, false };
        private bool[] bTogglePin = { false, false, false };
        private bool bToggleYodare = false;
        private float[] fYotogiValue = { 100f, 100f, 100f};
        //private bool bTogglePupil = false;
        private bool bLoadMaid = false;
        private Vector3[] fPupilValueDef =  { new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f) };
        private float[] fPupilValue = { 0f, 1f };
        private string[] pupilSliderLabel = {"è„Ç∏ÇÁÇµ", "ëÂÇ´Ç≥"};
        private int[] iFaceBlend = { 0, 0 };
        private string[][] sFaceBlend = new string[2][];
        private string[] sEroFaceAnime =  {"-----","í èÌÇP","í èÌÇQ","í èÌÇR"
                                          ,"ãªï±ÇO","ãªï±ÇP","ãªï±ÇQ","ãªï±ÇR"
                                          ,"ÉÅÉ\ãÉÇ´","„µípÇP","„µípÇQ","„µípÇR"
                                          ,"åôà´ÇP","çDä¥ÇP","çDä¥ÇQ","çDä¥ÇR"
                                          ,"ãØÇ¶","ä˙ë“","ê‚í∏","ï˙êS"
                                          ,"â‰ñùÇP","â‰ñùÇQ","â‰ñùÇR","ê„ê”"
                                          ,"í…Ç›ÇP","í…Ç›ÇQ","í…Ç›ÇR","ê„ê”âıäy"
                                          ,"í…Ç›â‰ñùÇQ","í…Ç›â‰ñùÇR","‰rÇﬂí èÌ","‰rÇﬂåôà´"
                                          ,"ÉtÉFÉâà§èÓ","ÉtÉFÉââıäy","ÉtÉFÉâåôà´","ÉtÉFÉâí èÌ"
                                          ,"‰rÇﬂà§èÓ","‰rÇﬂà§èÓÇQ","‰rÇﬂâıäy","‰rÇﬂâıäyÇQ"
                                          };
        private YotogiParamBasicBar yotogiParamBasicBar;
        delegate void SetValue(int val);
        delegate void SetValueBool(int val, bool f);
        

         private class PixelValues
        {
            public float BaseWidth = 1280f;
            public float PropRatio = 0.6f;
            public int Margin;

            private Dictionary<string, int> font = new Dictionary<string, int>();
            private Dictionary<string, int> line = new Dictionary<string, int>();
            private Dictionary<string, int> sys =  new Dictionary<string, int>();

            public PixelValues()
            {
                Margin = PropPx(10);

                font["C1"] = 11;
                 font["C2"] = 12;
                font["H1"] = 14;
                font["H2"] = 16;
                font["H3"] = 20;

                line["C1"] = 14;
                line["C2"] = 18;
                line["H1"] = 22;
                line["H2"] = 24;
                line["H3"] = 30;

                sys["Menu.Height"] = 45;
                sys["OkButton.Height"] = 95;

                sys["HScrollBar.Width"] = 15;
            }
            
            public int Font(string key) { return PropPx(font[key]); }
            public int Line(string key) { return PropPx(line[key]); }
            public int Sys(string key)  { return PropPx(sys[key]); }

            public int Font_(string key) { return font[key]; }
            public int Line_(string key) { return line[key]; }
            public int Sys_(string key)  { return sys[key]; }

            public Rect PropScreen(float left, float top, float width, float height)
            {
                return new Rect((int)((Screen.width - Margin * 2) * left + Margin)
                               ,(int)((Screen.height - Margin * 2) * top + Margin)
                               ,(int)((Screen.width - Margin * 2) * width)
                               ,(int)((Screen.height - Margin * 2) * height));
            }

            public Rect PropScreenMH(float left, float top, float width, float height)
            {
                Rect r = PropScreen(left, top, width, height);
                r.y += Sys("Menu.Height");
                r.height -= (Sys("Menu.Height") + Sys("OkButton.Height"));
                
                return r;
            }

            public Rect PropScreenMH(float left, float top, float width, float height, Vector2 last)
            {
                Rect r = PropScreen((float)(left/(last.x - Margin * 2)), (float)(top/(last.y - Margin * 2)), width, height);
                r.height -= (Sys("Menu.Height") + Sys("OkButton.Height"));
                
                return r;
            }

            public Rect InsideRect(Rect rect) 
            {
                return new Rect(Margin, Margin, rect.width - Margin * 2, rect.height - Margin * 2);
            }

            public Rect InsideRect(Rect rect, int height) 
            {
                return new Rect(Margin, Margin, rect.width - Margin * 2, height);
            }

            public int PropPx(int px) 
            {
                return (int)(px * (1f + (Screen.width/BaseWidth - 1f) * PropRatio));
            }
        }


    //--------

        public void Awake()
       {
            pv = new PixelValues();
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            sFaceBlend[0] = new string[] {"ñjÇO", "ñjÇP", "ñjÇQ", "ñjÇR"};
            sFaceBlend[1] = new string[] {"ó‹ÇO", "ó‹ÇP", "ó‹ÇQ", "ó‹ÇR"};
       }

        public void OnLevelWasLoaded(int level)
        {
            sceneLevel = level;

            if (sceneLevel == 9)
            {

            }
            else if (sceneLevel == 14) 
            {
                for(int i=0; i<bToggleEnabled.Length; i++) bToggleEnabled[i] = false;
                for(int i=0; i<bTogglePin.Length; i++)     bTogglePin[i]     = false;
                bToggleYodare = false;

                winRect = pv.PropScreenMH(1f - yotogiSliderWidth, 1f - yotogiSliderHeight, yotogiSliderWidth-0.02f, yotogiSliderHeight);
            }
            else visible = false;
        }

        void Update()
        {
            if (sceneLevel == 9)
            {

            }
            else if (sceneLevel == 14)
            {
                if (Input.GetKeyDown(KeyCode.F5)) visible = !visible;

                if (!yotogiParamBasicBar)
                {
                    yotogiParamBasicBar = getYotogiParamBasicBar();
                }
            }
            else 
            {
                if(visible) visible = false;
            }
        }
        
        public void OnGUI()
        {
            if (!visible) return;

            GUIStyle winStyle = "box";
            winStyle.fontSize = pv.Font("C1");
            winStyle.alignment = TextAnchor.UpperRight;

            if (sceneLevel == 9)
            {

            }
            else if (sceneLevel == 14 && yotogiParamBasicBar)
            {
                maid =  GameMain.Instance.CharacterMgr.GetMaid(0);
                if (maid == null) return;
            
                if (maid.body0.trsEyeL.localPosition != null && (!bLoadMaid))
                {
                    fPupilValueDef[0] = maid.body0.trsEyeL.localPosition;
                    fPupilValueDef[1] = maid.body0.trsEyeL.localScale;
                    bLoadMaid = true;
                }

                if (lastScreenSize != new Vector2(Screen.width, Screen.height))
                {
                    winRect = pv.PropScreenMH(winRect.x, winRect.y, yotogiSliderWidth, yotogiSliderHeight, lastScreenSize);
                    lastScreenSize = new Vector2(Screen.width, Screen.height);
                }

                winRect.height = pv.Font("H1") + pv.Line("C2") * (10 + Mathf.CeilToInt(sEroFaceAnime.Length / 4f)) + pv.Margin * 6;
                winRect = GUI.Window(1, winRect, addYotogiSlider, AddYotogiSlider.Version, winStyle);
            }
         }

    //--------

        private void addYotogiSlider(int winID)
        {
            Rect baseRect = pv.InsideRect(winRect);
            Rect headerRect = new Rect(baseRect.x, baseRect.y, baseRect.width, pv.Line("H1"));
            Rect conRect = new Rect(baseRect.x, baseRect.y + headerRect.height + pv.Margin
                                   ,baseRect.width, baseRect.height - headerRect.height - pv.Margin);
            GUIStyle lStyle = "label";
            GUIStyle tStyle = "toggle";
            GUIStyle btnStyle = "button";
            SetValue svFunc;
            SetValueBool svbFunc;

            tStyle.fontSize = pv.Font("C2");
            tStyle.alignment = TextAnchor.MiddleLeft;

            lStyle.fontSize = pv.Font("H1");
            lStyle.alignment = TextAnchor.UpperLeft;
            drawWinHeader(headerRect, "Yotogi Slider", lStyle);

            GUI.BeginGroup(conRect);
            {
                Rect outRect = new Rect(0, 0, conRect.width, 0);
                
                // ñÈâæÉXÉâÉCÉ_Å[
                outRect.height =  pv.Line("C2") * 4;
                GUI.BeginGroup(outRect);
                {
                    int min, max;
                    int lineWidth = (int)outRect.width;
                    Rect outRectC = new Rect(0, 0, 0, pv.Line("C2"));

                    //----
                    outRectC.width = lineWidth * 0.35f;
                    lStyle.fontSize = pv.Font("C2");
                    GUI.Label(outRectC, "Status : ", lStyle);
                    outRectC.x += outRectC.width;

                    outRectC.width = lineWidth * 0.35f;
                    tStyle.normal.textColor = bToggleEnabled[0] ? Color.white : Color.red;
                    tStyle.hover.textColor  = bToggleEnabled[0] ? Color.white : Color.red;
                    bToggleEnabled[0] = GUI.Toggle(outRectC, bToggleEnabled[0], bToggleEnabled[0] ? "Enabled" : "Disabled", tStyle);
                    outRectC.x = lineWidth * 0.85f;

                    outRectC.width = lineWidth * 0.15f;
                    lStyle.alignment = TextAnchor.LowerRight;
                    GUI.Label(outRectC, "å≈íË", lStyle);

                    outRectC.x = 0;
                    outRectC.y += outRectC.height;
                    //----
                    
                    lStyle.alignment = TextAnchor.LowerLeft;
                    for (int i=0; i<3; i++)
                    {    
                        switch(i)
                        {
                            case 0: 
                                min = -100;
                                max = 300;
                                svFunc = maid.Param.SetCurExcite;
                                svbFunc = yotogiParamBasicBar.SetCurrentExcite;
                                if(!bTogglePin[i]) fYotogiValue[i] = maid.Param.status.cur_excite;
                                break;
                            case 1:
                                min = 0;
                                max = maid.Param.status.mind + maid.Param.status.maid_class_bonus_status.mind;
                                svFunc = maid.Param.SetCurMind;
                                svbFunc = yotogiParamBasicBar.SetCurrentMind;
                                if(!bTogglePin[i]) fYotogiValue[i] = maid.Param.status.cur_mind;
                                break;
                            case 2: 
                                min = 0;
                                max = maid.Param.status.reason;
                                svFunc = maid.Param.SetCurReason;
                                svbFunc = yotogiParamBasicBar.SetCurrentReason;
                                if(!bTogglePin[i]) fYotogiValue[i] = maid.Param.status.cur_reason;
                                break;
                            default: 
                                min = 0;
                                max = 0;
                                svFunc = null;
                                svbFunc = null;
                                break;
                        }

                        outRectC.width = lineWidth * 0.3f;
                        GUI.Label(outRectC, yotogiSliderLabel[i] +" : "+ fYotogiValue[i], lStyle);
                        outRectC.x += outRectC.width;

                        outRectC.width = lineWidth * 0.6f;
                         fYotogiValue[i] = GUI.HorizontalSlider(outRectC, fYotogiValue[i], min, max);
                        outRectC.x += outRectC.width;

                        outRectC.y -= pv.PropPx(5);
                        outRectC.width = lineWidth * 0.1f;
                         bTogglePin[i] = GUI.Toggle(outRectC, bTogglePin[i], "");
                        outRectC.y += pv.PropPx(5);
                        
                        if(bToggleEnabled[0]) 
                        {
                            if(svFunc != null) svFunc((int)fYotogiValue[i]);
                            if(svbFunc != null) svbFunc((int)fYotogiValue[i], true);
                        }

                        outRectC.x = 0;
                        outRectC.y += outRectC.height;
                    }
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;


                // ìµ ÉeÉXÉg
                outRect.height =  pv.Line("C2") * 1;
                GUI.BeginGroup(outRect);
                {
                    float min, max;
                    int lineWidth = (int)outRect.width;
                    Rect outRectC = new Rect(0, 0, 0, pv.Line("C2"));

                    /*//----
                    outRectC.width = lineWidth * 0.35f;
                    lStyle.fontSize = pv.Font("C2");
                    GUI.Label(outRectC, "ìµ : ", lStyle);
                    outRectC.x += outRectC.width;

                    outRectC.width = lineWidth * 0.35f;
                    tStyle.normal.textColor = bTogglePupil ? Color.white : Color.red;
                    tStyle.hover.textColor  = bTogglePupil ? Color.white : Color.red;
                    bTogglePupil = GUI.Toggle(outRectC, bTogglePupil, bTogglePupil ? "Enabled" : "Disabled", tStyle);

                    outRectC.x = 0;
                    outRectC.y += outRectC.height;
                    //----*/

                    for (int i=0; i<1; i++)
                    {
                        switch(i)
                        {
                            case 0: 
                                min = 0f;
                                max = 1f;
                                break;
                            case 1:
                                min = 0.1f;
                                max = 1.0f;
                                break;
                            default: 
                                min = 0;
                                max = 0;
                                break;
                        }
                        
                        
                        outRectC.width = lineWidth * 0.3f;
                        GUI.Label(outRectC, "ìµÅ™ : "+ (fPupilValue[i]*100f).ToString("F0"), lStyle);
                        outRectC.x += outRectC.width;

                        outRectC.width = lineWidth * 0.6f;
                         fPupilValue[i] = GUI.HorizontalSlider(outRectC, fPupilValue[i], min, max);
                        outRectC.x += outRectC.width;

                        outRectC.x = 0;
                        outRectC.y += outRectC.height;
                    }

                    //if(bTogglePupil) 
                    //{
                        maid.body0.trsEyeL.localPosition = fPupilValueDef[0] + new Vector3(0f, fPupilValue[0]/50f, 0f);
                        maid.body0.trsEyeR.localPosition = fPupilValueDef[0] - new Vector3(0f, fPupilValue[0]/50f, 0f);
                        //maid.body0.trsEyeL.localScale = Vector3.Scale(fPupilValueDef[1], new Vector3(1f, fPupilValue[1], fPupilValue[1]));
                        //maid.body0.trsEyeR.localScale = Vector3.Scale(fPupilValueDef[1], new Vector3(1f, fPupilValue[1], fPupilValue[1]));
                    /*}
                    else
                    {
                        maid.body0.trsEyeL.localPosition = fPupilValueDef[0];
                        maid.body0.trsEyeR.localPosition = fPupilValueDef[0];
                        //maid.body0.trsEyeL.localScale = fPupilValueDef[1];
                        //maid.body0.trsEyeR.localScale = fPupilValueDef[1];
                        bLoadMaid = false;
                    }*/
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;


                // FaceBlend
                outRect.height =  pv.Line("C2") * 3;
                GUI.BeginGroup(outRect);
                {
                    string tmp = "";
                    int lineWidth = (int)outRect.width;
                    Rect outRectC = new Rect(0, 0, 0, pv.Line("C2"));

                    //----
                    outRectC.width = lineWidth * 0.35f;
                    GUI.Label(outRectC, "FaceBlend :", lStyle);
                    outRectC.x += outRectC.width;

                    outRectC.width = lineWidth * 0.35f;
                    tStyle.fontSize = pv.Font("C2");
                    tStyle.alignment = TextAnchor.MiddleLeft;
                    tStyle.normal.textColor = (bToggleEnabled[1]) ? Color.white : Color.red;
                    tStyle.hover.textColor  = (bToggleEnabled[1]) ? Color.white : Color.red;
                    bToggleEnabled[1] = GUI.Toggle(outRectC, bToggleEnabled[1], (bToggleEnabled[1]) ? "Enabled" : "Disabled", tStyle);
                    outRectC.x += outRectC.width;

                    outRectC.width = lineWidth * 0.30f;
                    tStyle.fontSize = pv.Font("C2");
                    tStyle.alignment = TextAnchor.MiddleLeft;
                    tStyle.normal.textColor = (bToggleYodare) ? Color.white : Color.white;
                    tStyle.hover.textColor  = (bToggleYodare) ? Color.white : Color.white;
                    bToggleYodare = GUI.Toggle(outRectC, bToggleYodare, (bToggleYodare) ? "ÇÊÇæÇÍóL" : "ÇÊÇæÇÍñ≥", tStyle);

                    outRectC.x = 0;
                    outRectC.y += outRectC.height;
                    //----

                    outRectC.width = lineWidth;
                    btnStyle.fontSize = pv.Font("C2");
                    for(int i=0; i<iFaceBlend.Length; i++)
                    {
                        iFaceBlend[i] = GUI.SelectionGrid(outRectC, iFaceBlend[i], sFaceBlend[i], 4, btnStyle);
                        tmp += sFaceBlend[i][iFaceBlend[i]];
                        outRectC.y += outRectC.height;
                    }

                    if(bToggleEnabled[1]) maid.FaceBlend((bToggleYodare) ? tmp + "ÇÊÇæÇÍ" : tmp);
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;


                // FaceAnime
                outRect.height =  pv.Line("C2") * (2 + Mathf.CeilToInt(sEroFaceAnime.Length / 4f));
                GUI.BeginGroup(outRect);
                {
                    int lineWidth = (int)outRect.width;
                    Rect outRectC = new Rect(0, 0, 0, pv.Line("C2"));

                    //----
                    outRectC.width = lineWidth * 0.35f;
                    tStyle.normal.textColor = Color.white;
                    GUI.Label(outRectC, "FaceAnime :", lStyle);
                    outRectC.x += outRectC.width;
                    /*
                    outRectC.width = lineWidth * 0.65f;
                    tStyle.fontSize = pv.Font("C2");;
                    tStyle.alignment = TextAnchor.MiddleLeft;
                    tStyle.normal.textColor = (bToggleEnabled[2]) ? Color.white : Color.red;
                    tStyle.hover.textColor  = (bToggleEnabled[2]) ? Color.white : Color.red;
                    bToggleEnabled[2] = GUI.Toggle(outRectC, bToggleEnabled[2], (bToggleEnabled[2]) ? "LipSync STOP" : "LipSync On", tStyle);
                    if(bToggleEnabled[2]) maid.StopKuchipakuPattern();
                    */
                    outRectC.x = 0;
                    outRectC.y += outRectC.height;
                    //----

                    outRectC.width = lineWidth * 0.25f;
                    outRectC.y += drawEroFaceAnimeButton(outRectC, maid) * outRectC.height;
                }
                GUI.EndGroup();
            }
            GUI.EndGroup();
            GUI.DragWindow();
        }
        

        private void drawWinHeader(Rect rect, string s, GUIStyle style)
        {
            GUI.Label(rect, s, style);
            {
                ;
            }
        }

    //--------

        private int drawEroFaceAnimeButton(Rect rect, Maid maid)
        {
            int j = 0;
            for(int i = 0; i<sEroFaceAnime.Length; i++)
            {
                if(GUI.Button(rect, sEroFaceAnime[i]))
                {
                    maid.FaceAnime("ÉGÉç" + sEroFaceAnime[i], 1f, 0);
                }

                if((i + 1) % 4 == 0)
                {
                    rect.x -= rect.width*3;
                    rect.y += rect.height;
                    if (j == 4 || j == 7) rect.y += pv.PropPx(5);
                    j++;
                } 
                else rect.x += rect.width;
            }
            
            return j;
        }

    //--------

        private YotogiParamBasicBar getYotogiParamBasicBar()
        {
            YotogiParamBasicBar ypbb = BaseMgr<YotogiParamBasicBar>.Instance;
            if (ypbb == null) return null;

            return ypbb;
        }

    }

}