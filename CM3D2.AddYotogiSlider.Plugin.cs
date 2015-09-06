using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityInjector.Attributes;

namespace CM3D2.AddYotogiSlider.Plugin
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginName("CM3D2 AddYotogiSlider"),
    PluginVersion("0.0.1.1")]
    public class AddYotogiSlider : UnityInjector.PluginBase
    {
        public const string PluginName = "AddYotogiSlider";
        public const string Version = "0.0.1.1";

        private int   sceneLevel;
        private bool  visible            = false;
        private bool  initCompleted      = false;
        private float fPassedTimeOnLevel = 0f;
        private float fLastInitTime      = 0f;
        private bool  writeLog           = false;
        //private float laslWriteLogTime   = 0f;

        private PixelValues pv;
        private string[] sliderName = {"興奮", "精神", "理性", "瞳↑","腹"};
        private Dictionary<string, SliderParam> sp = new Dictionary<string, SliderParam>();
        private Dictionary<string, PlayAnime>   pa = new Dictionary<string, PlayAnime>();

        private Rect    winRect;
        private Vector2 lastScreenSize;
        private float   winWidthRatio  = 0.20f;
        private float   winHeightRatio = 0.75f;
        private float[] fWinAnimeFrom;
        private float[] fWinAnimeTo;
        private Vector2 scrollViewVector = Vector2.zero;
        
        private string[] sKey =  { "WIN", "STATUS", "AHE", "BOTE", "FACEBLEND", "FACEANIME"};
        private Dictionary<string, string> sHeaderLabel = new Dictionary<string, string>()
        { {"WIN", AddYotogiSlider.Version}, {"STATUS", "Status :"}, {"AHE", "AutoAHE :"}
        , {"BOTE", "AutoBOTE :"}, {"FACEBLEND", "FaceBlend :"}, {"FACEANIME", "FaceAnime :"}};
        private Dictionary<string, bool>   bEnabled     = new Dictionary<string, bool>();
        private Dictionary<string, bool>   bVisible     = new Dictionary<string, bool>();
        
        private bool bToggleAheSliderVisible = false;
        private bool bToggleHaraSliderVisible = false;

        private int  iOrgasmCount = 0;

        //AutoAHE
        private int      idxAheOrgasm()      { return (int)Math.Min(Mathf.Floor(iOrgasmCount / 3f), 2); } //絶頂回数3,6,9で変化
        private int[]    iAheExcite          = new int[] { 275, 250, 225 };                               //適用の興奮閾値
        private float    fAheLastEye         = 0f;
        private float[]  fAheNormalEyeMax    = new float[] { 40f, 45f, 50f };                             //通常時の瞳の最大値
        private float[]  fAheOrgasmEyeMax    = new float[] { 50f, 60f, 70f };                             //絶頂時の瞳の最大値
        private float[]  fAheOrgasmEyeMin    = new float[] { 30f, 35f, 40f };                             //絶頂時の瞳の最小値
        private string[] fAheOrgasmFaceAnime = new string[] { "エロ放心", "エロ好感３", "通常射精後１" }; //絶頂時のFaceAnime
        private string[] fAheOrgasmFaceBlend = new string[] { "頬１涙１", "頬２涙２", "頬３涙３よだれ" }; //E絶頂時のFaceBlend

        //AutoBOTE
        private int iHaraIncrement = 10; //一回の腹の増加値
        private int iBoteHaraMax  = 100; //腹の最大値
        private int iBoteCount = 0;

        //AutoBlend
        private bool bToggleYodare = false;
        private int[] iFaceBlend = { 0, 0 };
        private string[][] sFaceBlend  = { new string[] {"頬０", "頬１", "頬２", "頬３"}
                                         , new string[] {"涙０", "涙１", "涙２", "涙３"} };

        //AutoAnime
        private string[] sFaceAnime = 
        {
        "エロ通常１", "エロ通常２", "エロ通常３","エロ羞恥１", "エロ羞恥２", "エロ羞恥３",
        "エロ興奮０","エロ興奮１", "エロ興奮２", "エロ興奮３", "エロ緊張", "エロ期待",
        "エロ好感１", "エロ好感２", "エロ好感３","エロ我慢１", "エロ我慢２", "エロ我慢３",
        "エロ嫌悪１",  "エロ怯え", "エロ痛み１", "エロ痛み２", "エロ痛み３", "エロメソ泣き",
        "エロ絶頂",  "エロ痛み我慢", "エロ痛み我慢２","エロ痛み我慢３", "エロ放心", "発情",
        "通常射精後１", "通常射精後２","興奮射精後１", "興奮射精後２","絶頂射精後１", "絶頂射精後２",
        "エロ舐め愛情", "エロ舐め愛情２", "エロ舐め快楽", "エロ舐め快楽２", "エロ舐め嫌悪", "エロ舐め通常",
        "閉じ舐め愛情", "閉じ舐め快楽", "閉じ舐め快楽２", "閉じ舐め嫌悪", "閉じ舐め通常","接吻", 
        "エロフェラ愛情", "エロフェラ快楽", "エロフェラ嫌悪", "エロフェラ通常","エロ舌責", "エロ舌責快楽",
        "閉じフェラ愛情", "閉じフェラ快楽", "閉じフェラ嫌悪", "閉じフェラ通常","閉じ目","目口閉じ",
        "通常", "怒り", "笑顔", "微笑み", "悲しみ２", "泣き",  
        "きょとん", "ジト目","あーん", "ためいき", "ドヤ顔", "にっこり", 
        "びっくり", "ぷんすか", "まぶたギュ", "むー", "引きつり笑顔", "疑問",
        "苦笑い", "困った", "思案伏せ目", "少し怒り", "誘惑",  "拗ね", 
        "優しさ","居眠り安眠","目を見開いて","痛みで目を見開いて", "余韻弱","目口閉じ",
        "恥ずかしい","照れ", "照れ叫び","ダンスウインク", "ダンスキス", "ダンスジト目",
        "ダンス困り顔", "ダンス真剣", "ダンス微笑み","ダンス目とじ", "ダンス憂い","ダンス誘惑", 
        "頬０涙０", "頬０涙１", "頬０涙２", "頬０涙３", "頬１涙０", "頬１涙１", 
        "頬１涙２", "頬１涙３", "頬２涙０", "頬２涙１", "頬２涙２", "頬２涙３", 
        "頬３涙１", "頬３涙０", "頬３涙２", "頬３涙３","頬３涙０よだれ", "頬３涙１よだれ", 
        "頬３涙２よだれ", "頬３涙３よだれ" 
        };
        private string sNowFace = "";

        private Maid maid;
        private string commandUnitName = "/UI Root/YotogiPlayPanel/CommandViewer/SkillViewer/MaskGroup/SkillGroup/CommandParent/CommandUnit";
        private GameObject goCommandUnit;
        private YotogiPlayManager yotogiPlayManager;
        private Action<Yotogi.SkillData.Command.Data> onClickCommand;
        private KagScript kagScript;
        private Func<KagTagSupport, bool> tagFace;
        private bool tagFaceOverride = false;

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

        private class SliderParam
        {
            public AddYotogiSlider Parent = null;
            public string Name;
            public float Value;
            public float Vmin;
            public float Vmax;
            public float Vscale;
            public float Vdef;
            public bool PinEnabled;
            public bool Pin;

            private Func<float>   getMaidValue;
            private Action<float> setMaidValue;

            public SliderParam(AddYotogiSlider p, string name)
            {
                Parent = p;
                Name = name;
                
                switch(Name)
                {
                    case "興奮" :
                        Value      = 0f;
                        Vmin       = -100f;
                        Vmax       = 300f;
                        Vscale     = 1f;
                        PinEnabled = true;
                    break;

                    case "精神" :
                        Value      = 0f;
                        Vmin       = 0f;
                        Vmax       = 0f;
                        Vscale     = 1f;
                        PinEnabled = true;
                    break;

                    case "理性" :
                        Value      = 0f;
                        Vmin       = 0f;
                        Vmax       = 0f;
                        Vscale     = 1f;
                        PinEnabled = true;
                    break;

                    case "瞳↑" :
                        Value      = 0f;
                        Vmin       = 0f;
                        Vmax       = 100f;
                        Vscale     = 5000f;
                        PinEnabled = false;
                    break;

                    case "腹" :
                        Value      = 0f;
                        Vmin       = 0f;
                        Vmax       = 200f;
                        Vscale     = 1f;
                        PinEnabled = false;
                    break;

                    default: 
                    break;
                }
                
                Vdef         = 0f;
                Pin          = false;
                getMaidValue = null; 
                setMaidValue = null;
            }
            
            public bool InitOnUpdate()
            {
                Maid maid = GameMain.Instance.CharacterMgr.GetMaid(0);
                YotogiParamBasicBar ypb = getInstance<YotogiParamBasicBar>();

                if (maid && ypb)
                {
                    switch(Name)
                    {
                        case "興奮" :
                            getMaidValue = ( ) => (float)maid.Param.status.cur_excite; 
                            setMaidValue = (x) =>
                            {
                                maid.Param.SetCurExcite((int)x); 
                                ypb.SetCurrentExcite((int)x, true);
                            };
                        break;

                        case "精神" :
                            Vmax = (float)maid.Param.status.mind;
                            getMaidValue = ( ) => (float)maid.Param.status.cur_mind; 
                            setMaidValue = (x) =>
                            {
                                maid.Param.SetCurMind((int)x); 
                                ypb.SetCurrentMind((int)x, true);
                            };
                        break;

                        case "理性" :
                            Vmax = (float)maid.Param.status.reason;
                            getMaidValue = ( ) => (float)maid.Param.status.cur_reason; 
                            setMaidValue = (x) =>
                            {
                                maid.Param.SetCurReason((int)x); 
                                ypb.SetCurrentReason((int)x, true);
                            };
                        break;

                        case "瞳↑" :
                            Vdef = maid.body0.trsEyeL.localPosition.y;
                            getMaidValue = ( ) => maid.body0.trsEyeL.localPosition.y;
                            setMaidValue = (x) => 
                            {
                                try {
                                Vector3 vl = maid.body0.trsEyeL.localPosition;
                                Vector3 vr = maid.body0.trsEyeR.localPosition;
                                maid.body0.trsEyeL.localPosition = new Vector3(vl.x, Math.Max(Vdef + x, 0f), vl.z);
                                maid.body0.trsEyeR.localPosition = new Vector3(vl.x, Math.Min(Vdef - x, 0f), vl.z);
                                } catch(Exception ex) { Debug.LogError(AddYotogiSlider.PluginName +" : "+ ex);}
                            };
                        break;

                        case "腹" :    
                            Vdef = (float)maid.GetProp("Hara").value;
                            getMaidValue = ( ) => (float)((maid.GetProp("Hara")).value);
                            setMaidValue = (x) => 
                            {
                                try {
                                maid.SetProp("Hara", (int)x, false);
                                maid.body0.VertexMorph_FromProcItem("hara", x/100f);
                                } catch { /*Debug.LogError(AddYotogiSlider.PluginName +" : "+ ex);*/}
                            };
                        break;
                        default: break;
                    }
                    
                    return (!isNull(getMaidValue) && !isNull(setMaidValue)) ?  true :  false;
                }
                else return false;
            }
            

            public void GetMaidValue()
            {
                if(!Pin) Value = getMaidValue() * Vscale; 
            }

            public void GetMaidValue(out float x)
            {
                x = getMaidValue() * Vscale; 
            }

            public void SetMaidValue()
            {
                setMaidValue(Value / Vscale);
            }

            public void SetMaidValue(float x)
            {
                Value = x;
                setMaidValue(Value / Vscale);
            }

        }

        private class PlayAnime
        {
            public AddYotogiSlider Parent = null;
            public string Key  = "";
            public string Name = "";
            public float[] Value;
            public bool NowPlaying { get{ return play && (passedTime < finishTime); } }
            public bool Finished   { get{ return (passedTime >= finishTime); } }
            public bool SetterExist{ get{ return (num == 1) ? !isNull(setValue0) : !isNull(setValue); } }

            private float[] vFrom;
            private float[] vTo;
            private int   type       = 1;
            private int   num        = 1;
            private bool  play       = false;
            private float passedTime = 0f;
            private float startTime  = 0f;
            private float finishTime = 0f;
            //private float[] actionTime;

            private Func<float>     progress  = null;
            private Action<float>   setValue0 = null;
            private Action<float[]> setValue  = null;
            
            public PlayAnime(AddYotogiSlider p, string name, int n, float st, float ft)
            {
                Parent      = p; 
                Name        = name; 
                Key         = (name.Split('.'))[0];
                num         = n;
                Value       = new float[n];
                vFrom       = new float[n];
                vTo         = new float[n];
                startTime   = st;
                finishTime  = ft;
                progress    = ( ) => (passedTime - startTime) / (finishTime - startTime);
            }

            public PlayAnime(AddYotogiSlider p, string name, int n, float st, float ft, int t)
            {
                Parent      = p; 
                Name        = name; 
                Key         = (name.Split('.'))[0];
                type        = t;
                num         = n;
                Value       = new float[n];
                vFrom       = new float[n];
                vTo         = new float[n];
                startTime   = st;
                finishTime  = ft;
                progress    = ( ) => (passedTime - startTime) / (finishTime - startTime);
             }

            public bool IsKye(string s)    { return s == Key; }
            public bool Contains(string s) { return Name.Contains(s); }

            public void SetFrom(float vform)            { vFrom[0] = vform; }
            public void SetTo(float vto)                { vTo[0]   = vto; } 
            public void SetSetter(Action<float> func)   { setValue0 = func; }
            public void Set(float vform, float vto)     { SetFrom(vform); SetTo(vto); } 

            public void SetFrom(float[] vform)          { if(vform.Length == num) Array.Copy(vform ,vFrom, num); } 
            public void SetTo(float[] vto)              { if(vto.Length   == num) Array.Copy(vto,   vTo,   num); }
            public void SetSetter(Action<float[]> func) { setValue = func; } 
            public void Set(float[] vform, float[] vto) { SetFrom(vform); SetTo(vto); } 

            public void Play()
            {
                if (SetterExist)
                {
                    passedTime = 0f;
                    play = true;
                }
            } 
            public void Play(float vform, float vto)     { Set(vform, vto); Play(); }
            public void Play(float[] vform, float[] vto) { Set(vform, vto); Play(); }

            public void Stop() { play = false; } 

            public void Update()
            {
                if (play)
                {
                    bool change = false;
                    
                    for(int i=0; i<num; i++)
                    {
                        if (vFrom[i] == vTo[i]) continue;
                        
                        if (passedTime >= finishTime)
                        {
                            Stop();
                        }
                        else if (passedTime >= startTime)
                        {
                            switch (type)
                            {
                                case 1 :
                                {
                                    Value[i] = vFrom[i] + (vTo[i] - vFrom[i]) * progress();
                                    change = true;
                                }
                                break;
                                case 2 :
                                {
                                    Value[i] = vFrom[i] + (vTo[i] - vFrom[i]) * Mathf.Pow(progress(), 2);
                                    change = true;
                                }
                                break;
                                default : break;
                            }
                            if(Parent.writeLog) Debug.LogError("PlayAnime["+Name+"].Update :"+ Value[i]);
                        }
                    }

                    if (change)
                    {
                        if(num == 1) setValue0(Value[0]);
                        else         setValue(Value);
                    }
                }

                passedTime += Time.deltaTime;
            }
        }
        
    //--------

        public void Awake()
        {
            pv = new PixelValues();
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            foreach (string key in sKey) bEnabled[key] = false;
            foreach (string sn in sliderName) sp[sn] = new SliderParam(this, sn);

            pa["WIN.Load"]    = new PlayAnime(this, "WIN.Load",    2, 0.00f,  0.25f, 2);
            pa["AHE.継続.0"]  = new PlayAnime(this, "AHE.継続.0",  1, 0.00f,  0.75f);
            pa["AHE.絶頂.0"]  = new PlayAnime(this, "AHE.絶頂.0",  1, 6.00f,  9.00f);
            pa["BOTE.絶頂"]   = new PlayAnime(this, "BOTE.絶頂",   1, 0.00f,  6.00f);
            pa["BOTE.止める"] = new PlayAnime(this, "BOTE.止める", 1, 0.00f,  4.00f);
        }
        

        public void OnLevelWasLoaded(int level)
        {
            sceneLevel = level;
            visible = false;

            if (sceneLevel == 14) 
            {
                initCompleted = false;
                fPassedTimeOnLevel = 0f;
            } 
            else if (tagFaceOverride)
            {
                kagScript.RemoveTagCallBack("face");
                kagScript.AddTagCallBack("face", new KagScript.KagTagCallBack(this.tagFace));
                tagFaceOverride = false;
            }
        }

        public void Update()
        {
            fPassedTimeOnLevel += Time.deltaTime;

            if (sceneLevel == 14)
            {
                if (!initCompleted && (fPassedTimeOnLevel - fLastInitTime > 1f))
                { 
                    initCompleted = init();
                    fLastInitTime = fPassedTimeOnLevel;
                }
                
                if (!canStart()) return;

                if (Input.GetKeyDown(KeyCode.F5))
                {
                    if (!visible) winRect = pv.PropScreenMH(1f - winWidthRatio, 1f - winHeightRatio, winWidthRatio - 0.016f, winHeightRatio);
                    visible = !visible;
                    playAnimeOnInputKeyDown();
                }

                foreach(KeyValuePair<string, PlayAnime> o in pa) if (bEnabled[o.Value.Key]) o.Value.Update();
                if (pa["AHE.絶頂.0"].NowPlaying) maid.FaceBlend(fAheOrgasmFaceBlend[idxAheOrgasm()]);
            }
        }

        public void OnGUI()
        {
            if (sceneLevel == 14 && canStart())
            {
                if (pa["WIN.Load"].NowPlaying) 
                { 
                    pa["WIN.Load"].Update();
                    GUIStyle winStyle = "box";
                    winStyle.fontSize = pv.Font("C1");
                    winStyle.alignment = TextAnchor.UpperRight;
                    winRect = GUI.Window(1, winRect, dummyWin, AddYotogiSlider.Version, winStyle);
                }
                else if (visible)
                {
                    if (lastScreenSize != new Vector2(Screen.width, Screen.height))
                    {
                        winRect = pv.PropScreenMH(winRect.x, winRect.y, winWidthRatio, winHeightRatio, lastScreenSize);
                        lastScreenSize = new Vector2(Screen.width, Screen.height);
                    }
                    winRect.height = calcWinHeight();

                    GUIStyle winStyle = "box";
                    winStyle.fontSize = pv.Font("C1");
                    winStyle.alignment = TextAnchor.UpperRight;
                    winRect = GUI.Window(1, winRect, addYotogiSlider, sHeaderLabel["WIN"], winStyle);
                }
            }
         }

        public void OnYotogiPlayManagerOnClickCommand(Yotogi.SkillData.Command.Data command_data)
        {
            initAnimeOnCommand();
            onClickCommand(command_data);
            playAnimeOnCommand(command_data.basic);
            
            if (command_data.basic.command_type == Yotogi.SkillCommandType.絶頂) 
            {
                if (!bEnabled["FACEANIME"] && pa["AHE.絶頂.0"].NowPlaying)
                {
                    maid.FaceAnime(fAheOrgasmFaceAnime[idxAheOrgasm()], 3f, 0);
                    sNowFace = fAheOrgasmFaceAnime[idxAheOrgasm()];
                }
                iOrgasmCount++;
            }
        }

        public bool OnYotogiKagManagerTagFace(KagTagSupport tag_data)
        {
            if (bEnabled["FACEANIME"] || pa["AHE.絶頂.0"].NowPlaying) 
            {
                return false;
            }
            else 
            {
                sNowFace = tag_data.GetTagProperty("name").AsString();
                return tagFace(tag_data);
            }
        }

    //--------

        private bool init()
        {
            this.goCommandUnit = GameObject.Find(commandUnitName);
            if (!this.goCommandUnit) return false;

            this.maid = GameMain.Instance.CharacterMgr.GetMaid(0); 
            if (!this.maid) return false;
            
            // 夜伽コマンド
            this.yotogiPlayManager = getInstance<YotogiPlayManager>(); 
            if (yotogiPlayManager)
            {
                YotogiCommandFactory cf = getFieldValue<YotogiPlayManager, YotogiCommandFactory>(this.yotogiPlayManager, "command_factory_");
                if (!isNull(cf))
                {
                    try {
                        cf.SetCommandCallback(new YotogiCommandFactory.CommandCallback(this.OnYotogiPlayManagerOnClickCommand));
                    } catch(Exception ex) { Debug.LogError(AddYotogiSlider.PluginName +" SetCommandCallback() : "+ ex); return false; }
                }
                else return false;
                
                this.onClickCommand = getMethod<YotogiPlayManager, Action<Yotogi.SkillData.Command.Data>>(this.yotogiPlayManager, "OnClickCommand");
                if (isNull(this.onClickCommand)) return false;
            }
            else return false;
            
            // 夜伽FaceAnime
            YotogiKagManager ykm = GameMain.Instance.ScriptMgr.yotogi_kag;
            if (!isNull(ykm))
            {
                try{this.kagScript = getFieldValue<YotogiKagManager, KagScript>(ykm, "kag_");
                if (!isNull(this.kagScript))
                {
                    this.kagScript.RemoveTagCallBack("face");
                    this.kagScript.AddTagCallBack("face", new KagScript.KagTagCallBack(this.OnYotogiKagManagerTagFace));
                    tagFaceOverride = true;

                    this.tagFace = getMethod<YotogiKagManager, Func<KagTagSupport, bool>>(ykm, "TagFace");
                    if (isNull(this.tagFace)) return false;
                }    
                else return false;
                } catch(Exception ex) { Debug.LogError("failed : "+ ex); }
            }
            else return false;

            bool success = true;
            foreach(string sn in sliderName) success &= sp[sn].InitOnUpdate();
            if (!success) return false;
            
            foreach(KeyValuePair<string, PlayAnime> o in pa)
            {
                PlayAnime p = o.Value;
                if (!p.SetterExist) 
                {
                    if (p.Contains("WIN"))  p.SetSetter( (Action<float[]>)setWinValue );
                    if (p.Contains("AHE"))  p.SetSetter( (Action<float>)sp["瞳↑"].SetMaidValue );
                    if (p.Contains("BOTE")) p.SetSetter( (Action<float>)sp["腹"].SetMaidValue );
                }
            }
            

            foreach(string key in sKey) 
            {
                bEnabled[key] = false;
                bVisible[key] = false;
            }
            bToggleYodare = false;
            iBoteCount = 0;
            iOrgasmCount = 0;

            return true;
        }

        private void dummyWin(int winID)
        {
            
        }

        private void addYotogiSlider(int winID)
        {
            Rect baseRect = pv.InsideRect(winRect);
            Rect headerRect = new Rect(baseRect.x, baseRect.y, baseRect.width, pv.Line("H1"));
            Rect conRect = new Rect(baseRect.x, baseRect.y + (headerRect.height + pv.Margin)
                                   ,baseRect.width, baseRect.height - (headerRect.height + pv.Margin));
            GUIStyle lStyle = "label";
            GUIStyle tStyle = "toggle";
            GUIStyle btnStyle = "button";

            drawWinHeader(headerRect, "Yotogi Slider");

            GUI.BeginGroup(conRect);
            {
                Rect outRect = new Rect(0, 0, conRect.width, 0);
                
                // 夜伽スライダー
                outRect.height =  pv.Line("C2") * 4;
                GUI.BeginGroup(outRect);
                {
                    string key = "STATUS";
                    Rect outRectC = new Rect(0, 0, outRect.width, pv.Line("C2"));

                    drawHeaderToggle(outRectC, key);
                    outRectC.y += outRectC.height;

                    for (int i=0; i<3; i++)
                    {    
                        string sn = sliderName[i];
                        sp[sn].GetMaidValue();

                        drawSlider(outRectC, sn);
                        outRectC.x = 0;
                        outRectC.y += outRectC.height;

                        if(bEnabled[key]) sp[sn].SetMaidValue();
                    }
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;

                // Auto AHE + スライダー
                outRect.height =  pv.Line("C2") * (1 + (bToggleAheSliderVisible ? 1 : 0));
                GUI.BeginGroup(outRect);
                {
                    string key = "AHE";
                    Rect outRectC = new Rect(0, 0, outRect.width, pv.Line("C2"));

                    drawHeaderToggle(outRectC, key);
                    outRectC.y += outRectC.height;

                    if(bToggleAheSliderVisible)
                    {
                        string sn = sliderName[3];
                        sp[sn].GetMaidValue();
                        
                        drawSlider(outRectC, sn);
                        outRectC.y += outRectC.height;

                        sp[sn].SetMaidValue();
                    }
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;

                // Auto BOTE + スライダー
                outRect.height =  pv.Line("C2") * (1 + (bToggleHaraSliderVisible ? 1 : 0));
                GUI.BeginGroup(outRect);
                {
                    string key = "BOTE";
                    Rect outRectC = new Rect(0, 0, outRect.width, pv.Line("C2"));

                    drawHeaderToggle(outRectC, key);
                    outRectC.y += outRectC.height;
                    
                    if(bToggleHaraSliderVisible)
                    {
                        string sn = sliderName[4];
                        sp[sn].GetMaidValue();
                        
                        drawSlider(outRectC, sn);
                        outRectC.y += outRectC.height;

                        sp[sn].SetMaidValue();
                    }
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;

                // FaceBlend
                outRect.height =  pv.Line("C2") * 3;
                GUI.BeginGroup(outRect);
                {
                    string key = "FACEBLEND";
                    Rect outRectC = new Rect(0, 0, outRect.width, pv.Line("C2"));

                    drawHeaderToggle(outRectC, key);
                    outRectC.y += outRectC.height;

                    string tmp = "";
                    btnStyle.fontSize = pv.Font("C2");
                    for(int i=0; i<iFaceBlend.Length; i++)
                    {
                        iFaceBlend[i] = GUI.SelectionGrid(outRectC, iFaceBlend[i], sFaceBlend[i], 4, btnStyle);
                        tmp += sFaceBlend[i][iFaceBlend[i]];
                        outRectC.y += outRectC.height;
                    }

                    if(bEnabled[key]) maid.FaceBlend((bToggleYodare) ? tmp + "よだれ" : tmp);
                }
                GUI.EndGroup();
                outRect.x = 0;
                outRect.y += outRect.height + pv.Margin;

                // FaceAnime
                outRect.height =  pv.Line("C2") * 7 + pv.Margin + pv.Sys_("HScrollBar.Width") + pv.PropPx(5);
                GUI.BeginGroup(outRect);
                {
                    string key = "FACEANIME";
                    Rect outRectC = new Rect(0, 0, outRect.width, pv.Line("C2"));

                    drawHeaderToggle(outRectC, key);
                    outRectC.y += outRectC.height;

                    Rect scrollRect = new Rect(0, outRectC.y, outRect.width, pv.Line("C2") * 6 + pv.PropPx(5));
                    Rect scrconRect = new Rect(0, 0, (outRectC.width - pv.Sys_("HScrollBar.Width") - pv.PropPx(5)), pv.Line("C2") * 61 + pv.PropPx(5) * 20);
                    scrollViewVector = GUI.BeginScrollView(scrollRect, scrollViewVector, scrconRect, false, true);
                    {    
                        outRectC.y = 0;
                        outRectC.width = scrconRect.width / 2;
                        drawFaceAnimeButton(outRectC);
                    }
                    GUI.EndScrollView();
                }
                GUI.EndGroup();
            }

            GUI.EndGroup();
            GUI.DragWindow();
        }

        private void drawWinHeader(Rect rect, string s)
        {
            GUIStyle lStyle = "label";
            lStyle.fontSize = pv.Font("H1");
            lStyle.alignment = TextAnchor.UpperLeft;

            GUI.Label(rect, s, lStyle);
            {
                ;
            }
        }

        private void drawHeaderToggle(Rect outRectC, string key)
        {
            string s = "";
            GUIStyle lStyle = "label";
            GUIStyle tStyle = "toggle";
            GUIStyle btnStyle = "button";
            float lineWidth = outRectC.width;

            outRectC.width = lineWidth * 0.33f;
            lStyle.fontSize = pv.Font("C2");
            lStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(outRectC, sHeaderLabel[key], lStyle);
            outRectC.x += outRectC.width;

            outRectC.y -= pv.PropPx(2);
            outRectC.width = lineWidth * 0.37f;
            tStyle.fontSize = pv.Font("C2");
            tStyle.alignment = TextAnchor.MiddleLeft;
            tStyle.normal.textColor = toggleColor(bEnabled[key]);
            tStyle.hover.textColor  = toggleColor(bEnabled[key]);
            if (key == "FACEANIME")
            {
                outRectC.width = lineWidth * 0.33f;
                s = bEnabled[key] ? "上書禁止" : "上書許可" ;
            }
            else s = toggleText(bEnabled[key]);
            bEnabled[key] = GUI.Toggle(outRectC, bEnabled[key], s, tStyle);
            outRectC.y += pv.PropPx(2);

            switch(key)
            {
                case "STATUS" :
                    outRectC.x = lineWidth * 0.85f;
                    outRectC.width = lineWidth * 0.15f;
                    lStyle.alignment = TextAnchor.LowerRight;
                    GUI.Label(outRectC, "固定", lStyle);
                break;
                case "AHE" : 
                    outRectC.x = lineWidth * 0.70f;
                    outRectC.width = lineWidth * 0.30f;
                    btnStyle.fontSize = pv.Font("C2");
                    bToggleAheSliderVisible = GUI.Toggle(outRectC, bToggleAheSliderVisible, "Slider", btnStyle);
                break;
                case "BOTE" : 
                    outRectC.x = lineWidth * 0.70f;
                    outRectC.width = lineWidth * 0.30f;
                    btnStyle.fontSize = pv.Font("C2");
                    bToggleHaraSliderVisible = GUI.Toggle(outRectC, bToggleHaraSliderVisible, "Slider", btnStyle);
                break;
                case "FACEBLEND" : 
                    outRectC.y -= pv.PropPx(2);
                    outRectC.x = lineWidth * 0.70f;
                    outRectC.width = lineWidth * 0.30f;
                    tStyle.normal.textColor = toggleColor(bToggleYodare);
                    tStyle.hover.textColor  = toggleColor(bToggleYodare);
                    bToggleYodare = GUI.Toggle(outRectC, bToggleYodare, (bToggleYodare) ? "よだれ有" : "よだれ無", tStyle);
                    outRectC.y += pv.PropPx(2);
                break;
                case "FACEANIME" :
                    outRectC.x = lineWidth * 0.66f;
                    outRectC.width = lineWidth * 0.34f;
                    GUI.Label(outRectC, sNowFace, lStyle);
                break;
                default : break;
            }
        }

        private void drawSlider(Rect outRectC, string sn)
        {
            GUIStyle lStyle = "label";
            float lineWidth = outRectC.width;

            outRectC.width = lineWidth * 0.30f;
            lStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(outRectC, sn +" : "+ sp[sn].Value.ToString("F0"), lStyle);
            outRectC.x += outRectC.width;

            outRectC.y += pv.PropPx(3);
            outRectC.width = lineWidth * 0.6f;
            sp[sn].Value = GUI.HorizontalSlider(outRectC, sp[sn].Value, sp[sn].Vmin, sp[sn].Vmax);
            outRectC.x += outRectC.width;
            outRectC.y -= pv.PropPx(3);

            if (sp[sn].PinEnabled) 
            {
                outRectC.y -= pv.PropPx(5);
                outRectC.width = lineWidth * 0.10f;
                sp[sn].Pin = GUI.Toggle(outRectC, sp[sn].Pin, "");
                outRectC.y += pv.PropPx(5);
            }
        }

        private void drawFaceAnimeButton(Rect rect)
        {
            for(int i = 0; i<sFaceAnime.Length; i++)
            {
                if(GUI.Button(rect, sFaceAnime[i]))
                {
                    maid.FaceAnime(sFaceAnime[i], 1f, 0);
                    sNowFace = sFaceAnime[i];
                }

                if((i + 1) % 2 == 0)
                {
                    rect.x = 0;
                    rect.y += rect.height;
                    if ((i + 1) % 6 == 0) rect.y += pv.PropPx(5);
                } 
                else rect.x += rect.width;
            }
        }

    //--------

        private bool canStart()
        {
            return initCompleted && isActive(goCommandUnit) && yotogiPlayManager.fade_status == WfScreenChildren.FadeStatus.Wait;
        }

        private float calcWinHeight()
        {
            return pv.Margin * 2 
                 + pv.Font("H1") + pv.Margin * 1 
                 + pv.Line("C2") * ( 4 + 1 + 1 + 3 + 7 + (bToggleAheSliderVisible ? 1 : 0) + (bToggleHaraSliderVisible ? 1 : 0) )
                 + pv.Margin * 5 + pv.PropPx(5);
        }
        
        private Color toggleColor(bool b)
        {
            return b ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.0f, 0.0f, 1f);
        }

        private string toggleText(bool b)
        {
            return b ? "Enabled" : "Disabled";
        }

    //--------

        private void initAnimeOnCommand()
        {
            if (bEnabled["AHE"]) 
            {
                fAheLastEye = sp["瞳↑"].Value;
                for (int i=0; i<1; i++)
                {
                    if (pa["AHE.絶頂."+ i].NowPlaying) pa["AHE.絶頂."+ i].Stop();
                    if (pa["AHE.継続."+ i].NowPlaying) pa["AHE.継続."+ i].Stop();
                }
            }

            if (bEnabled["BOTE"]) 
            {
                if (pa["BOTE.絶頂"].NowPlaying) 
                {
                    float hara = sp["腹"].Vdef + iHaraIncrement * iBoteCount;
                    sp["腹"].Value = Mathf.Min(hara, iBoteHaraMax);
                }
                if (pa["BOTE.止める"].NowPlaying) 
                {
                    sp["腹"].Value = sp["腹"].Vdef;
                }
                
                pa["BOTE.絶頂"].Stop();
                pa["BOTE.止める"].Stop();

            }
        }

        private void playAnimeOnCommand(Yotogi.SkillData.Command.Data.Basic data)
        {
            if (bEnabled["AHE"]) 
            {
                float excite;
                sp["興奮"].GetMaidValue(out excite);
                int i = idxAheOrgasm();
                
                if (data.command_type == Yotogi.SkillCommandType.絶頂)
                {
                    pa["AHE.継続.0"].Play(fAheLastEye,fAheOrgasmEyeMax[i]);
                    pa["AHE.絶頂.0"].Play(fAheOrgasmEyeMax[i], fAheOrgasmEyeMin[i]);
                }
                else
                {
                    if (excite >= iAheExcite[i])
                    {
                        float to = fAheNormalEyeMax[i] * (excite - iAheExcite[i]) / (300f - iAheExcite[i]);
                        pa["AHE.継続.0"].Play(fAheLastEye, to);
                    }
                }
            }

            if (bEnabled["BOTE"]) 
            {
                if (data.command_type == Yotogi.SkillCommandType.絶頂)
                {
                    if (data.name.Contains("中出し") || data.name.Contains("注ぎ込む"))
                    {
                        iBoteCount++;
                        float to = sp["腹"].Vdef + iHaraIncrement * iBoteCount;
                        to = Mathf.Min(to, iBoteHaraMax);
                        pa["BOTE.絶頂"].Play(sp["腹"].Value, to);
                    }
                    else if (data.name.Contains("外出し"))
                    {
                        pa["BOTE.止める"].Play(sp["腹"].Value, sp["腹"].Vdef);
                        iBoteCount = 0;
                    }
                }
                else if (data.command_type == Yotogi.SkillCommandType.止める)
                {
                    pa["BOTE.止める"].Play(sp["腹"].Value, sp["腹"].Vdef);
                    iBoteCount = 0;
                }
            }
        }

        private void playAnimeOnInputKeyDown()
        {
            winRect.height = calcWinHeight();
            if (visible)
            {
                fWinAnimeFrom = new float[2] { Screen.width, 0f };
                fWinAnimeTo   = new float[2] { winRect.x , 1f };
            }
            else
            {
                fWinAnimeFrom = new float[2] { winRect.x, 1f };
                fWinAnimeTo   = new float[2] { (winRect.x + winRect.width / 2> Screen.width / 2f) ? Screen.width : -winRect.width, 0f };
            }
            pa["WIN.Load"].Play(fWinAnimeFrom, fWinAnimeTo);
        }


        private void setWinValue(float[] x)
        {
            winRect.x = x[0];
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, x[1]);
        }
    

    //--------

        internal static void writeTrans(Transform t, int level, StreamWriter writer)
        {
            if (level == 0) writer = new StreamWriter(@".\"+ t.name +@".txt", false);
            if (writer == null) return;
            
            writer.WriteLine(level +","+t.name);
            foreach (Transform tc in t)
            {
                writeTrans(tc, level+1, writer);
            }

            if (level == 0) writer.Close();
        }

        internal static bool isNull<T>(T t) where T : class
        {
            return (t == null) ? true : false;
        }
        
        internal static bool isNull<T>(T t, string s) where T : class
        {
            if(t == null) Debug.LogError(AddYotogiSlider.PluginName +" : "+ s);
            return isNull<T>(t);
        }

        internal static bool goExist(string name)
        {
            return (GameObject.Find(name)) ? true : false;
        }

        internal static bool isActive(GameObject go)
        {
            return (go) ? go.activeInHierarchy : false;
        }
        
        internal static T getInstance<T>() where T : class
        {
            return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
        }

        internal static TResult getMethod<T, TResult>(T inst, string name) where T : class  where TResult : class
        {
            return Delegate.CreateDelegate(typeof(TResult), inst, name) as TResult;
        }

        internal static TResult getFieldValue<T, TResult>(T inst, string name) where T : class
        {
            BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            FieldInfo field = typeof(T).GetField(name, bf);
            if (field == null) return default(TResult);

            return (TResult)field.GetValue(inst);
        }

    }
}
