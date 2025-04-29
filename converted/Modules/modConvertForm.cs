using Microsoft.VisualBasic;
using System.Collections.Generic;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static modConfig;
using static modConvertUtils;
using static modUtils;
using static modVB6ToCS;
using static VBExtension;


static class modConvertForm
{
    // Option Explicit
    private static string EventStubs = "";


    public static string Frm2Xml(string F)
    {
        string Frm2Xml = "";
        List<string> Sp = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long
        List<dynamic> L = new List<dynamic> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long
        List<int> I = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long

        string R = "";

        Sp = Split(F, vbCrLf);

        foreach (var iterL in Sp)
        {
            L = iterL;
            L = Trim(L);
            if (L == "")
            {
                goto NextLine;
            }
            if (Left(L, 10) == "Attribute " || Left(L, 8) == "VERSION ")
            {
            }
            else if (Left(L, 6) == "Begin ")
            {
                R = R + sSpace(I * SpIndent) + "<item type=\"" + SplitWord(L, 2) + "\" name=\"" + SplitWord(L, 3) + "\">" + vbCrLf;
                I = I + 1;
            }
            else if (L == "End")
            {
                I = I - 1;
                R = R + sSpace(I * SpIndent) + "</item>" + vbCrLf;
            }
            else
            {
                R = R + sSpace(I * SpIndent) + "<prop name=\"" + SplitWord(L, 1, "=") + "\" value=\"" + SplitWord(L, 2, "=", true, true) + "\" />" + vbCrLf;
            }
        NextLine:;
        }
        Frm2Xml = R;
        return Frm2Xml;
    }

    public static string FormControls(string Src, string F, bool asLocal = true)
    {
        string FormControls = "";
        List<string> Sp = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long
        List<dynamic> L = new List<dynamic> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long
        List<int> I = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, I As Long

        string R = "";
        string T = "";

        string Nm = "";
        string Ty = "";

        Sp = Split(F, vbCrLf);

        foreach (var iterL in Sp)
        {
            L = iterL;
            L = Trim(L);
            if (L == "")
            {
                goto NextLine;
            }
            if (Left(L, 6) == "Begin ")
            {
                Ty = SplitWord(L, 2);
                Nm = SplitWord(L, 3);
                switch (Ty)
                {
                    case "VB.Form":
                        break;
                    default:
                        T = Src + ":" + IIf(asLocal, "", Src + ".") + Nm + ":Control:" + Ty;
                        if (Right(R, Len(T)) != T)
                        {
                            R = R + vbCrLf + T;
                        }
                        break;
                }
            }
        NextLine:;
        }
        FormControls = R;
        return FormControls;
    }

    public static string ConvertFormUi(string F, string CodeSection)
    {
        string ConvertFormUi = "";
        List<string> Stck = new List<string>(new string[1]);

        List<string> Sp = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String
        List<dynamic> L = new List<dynamic> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String
        List<int> J = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String
        List<int> K = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String
        List<int> I = new List<int> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String
        List<string> Tag = new List<string> { }; // TODO - Specified Minimum Array Boundary Not Supported:   Dim Sp() As String, L As Variant, J As Long, K As Long, I As Long, Tag As String

        string M = "";

        string R = "";

        string Prefix = "";

        Collection Props = null;
        string pK = "";
        string pV = "";

        Sp = Split(F, vbCrLf);

        EventStubs = "";

        for (K = LBound(Sp); K < UBound(Sp); K++)
        {
            L = Trim(Sp[K]);
            if (L == "")
            {
                goto NextLine;
            }

            if (Left(L, 10) == "Attribute " || Left(L, 8) == "VERSION ")
            {
            }
            else if (Left(L, 6) == "Begin ")
            {
                Props = new Collection(); ;
                J = 0;
                do
                {
                    J = J + 1;
                    M = Trim(Sp[K + J]);
                    if (LMatch(M, "Begin ") || M == "End")
                    {
                        break;
                    }

                    if (LMatch(M, "BeginProperty "))
                    {
                        Prefix = LCase(Prefix + SplitWord(M, 2) + ".");
                    }
                    else if (LMatch(M, "EndProperty"))
                    {
                        Prefix = Left(Prefix, Len(Prefix) - 1);
                        if (!IsInStr(Prefix, "."))
                        {
                            Prefix = "";
                        }
                        else
                        {
                            Prefix = Left(Prefix, InStrRev(Left(Prefix, Len(Prefix) - 1), "."));
                        }
                    }
                    else
                    {
                        pK = Prefix + LCase(SplitWord(M, 1, "="));
                        pV = ConvertProperty(SplitWord(M, 2, "=", true, true));
                        // TODO (not supported): On Error Resume Next
                        Props.Add(pV, pK);
                        // TODO (not supported): On Error GoTo 0
                    }
                } while (!(true));
                K = K + J - 1;
                R = R + sSpace(I * SpIndent) + StartControl(L, Props, LMatch(M, "End"), CodeSection, Tag) + vbCrLf;
                I = I + 1;
                Stck[I] = Tag;
            }
            else if (L == "End")
            {
                Props = null;
                Tag = Stck[I];
                I = I - 1;
                if (Tag != "")
                {
                    R = R + sSpace(I * SpIndent) + EndControl(Tag) + vbCrLf;
                }
            }
        NextLine:;
        }
        ConvertFormUi = R;
        return ConvertFormUi;
    }

    private static string ConvertProperty(string S)
    {
        string ConvertProperty = "";
        S = deQuote(S);
        S = DeComment(S);
        ConvertProperty = S;
        return ConvertProperty;
    }

    private static string StartControl(string L, Collection Props, bool DoEmpty, string Code, out string TagType)
    {
        string StartControl = "";
        string cType = "";
        string cName = "";
        string cIndex = "";

        string tType = "";
        bool tCont = false;
        string tDef = "";
        string Features = "";

        string S = "";
        string N = "";
        string M = "";

        string V = "";

        N = vbCrLf;
        TagType = "";

        cType = SplitWord(L, 2);
        cName = SplitWord(L, 3);
        cIndex = cValP(ref Props, "Index");
        if (cIndex != "")
        {
            cName = cName + "_" + cIndex;
        }

        ControlData(cType, tType, tCont, tDef, Features);

        S = "";
        // TODO (not supported): On Error Resume Next
        if (tType == "Line" || tType == "Shape" || tType == "Timer")
        {
            return StartControl;

        }
        else if (tType == "Window")
        {
            S = S + M + "<Window x:Class=\"" + AssemblyName() + ".Forms." + cName + "\"";
            S = S + N + "    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";
            S = S + N + "    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";
            S = S + N + "    xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"";
            S = S + N + "    xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"";
            S = S + N + "    xmlns:local=\"clr-namespace:" + AssemblyName() + ".Forms\"";
            S = S + N + "    xmlns:usercontrols=\"clr-namespace:" + AssemblyName() + ".UserControls\"";
            S = S + N + "    mc:Ignorable=\"d\"";
            S = S + N + "    Title=" + Quote(cValP(ref Props, "caption"));
            S = S + M + "    Height=" + Quote(Px(cValP(ref Props, "clientheight", 0) + 435));
            S = S + M + "    Width=" + Quote(Px(cValP(ref Props, "clientwidth", 0) + 435));
            S = S + CheckControlEvents("Window", "Form", Code);
            S = S + M + ">";
            S = S + N + " <Grid";
        }
        else if (tType == "GroupBox")
        {
            S = S + "<" + tType;
            S = S + " x:Name=\"" + cName + "\"";

            S = S + " Margin=" + Quote(Px(cValP(ref Props, "left")) + "," + Px(cValP(ref Props, "top")) + ",0,0");
            S = S + " Width=" + Quote(Px(cValP(ref Props, "width")));
            S = S + " Height=" + Quote(Px(cValP(ref Props, "height")));
            S = S + " VerticalAlignment=\"Top\"";
            S = S + " HorizontalAlignment=\"Left\"";
            S = S + " FontFamily=" + Quote(cValP(ref Props, "font.name", "Calibri"));
            S = S + " FontSize=" + Quote(cValP(ref Props, "font.size", 10));

            S = S + " Header=\"" + cValP(ref Props, "caption") + "\"";
            S = S + "> <Grid Margin=\"0,-15,0,0\"";
        }
        else if (tType == "Canvas")
        {
            S = S + "<" + tType;
            S = S + " x:Name=\"" + cName + "\"";

            S = S + " Margin=" + Quote(Px(cValP(ref Props, "left")) + "," + Px(cValP(ref Props, "top")) + ",0,0");
            S = S + " Width=" + Quote(Px(cValP(ref Props, "width")));
            S = S + " Height=" + Quote(Px(cValP(ref Props, "height")));
        }
        else if (tType == "Image")
        {
            S = S + "<" + tType;

            S = S + " x:Name=\"" + cName + "\"";
            S = S + " Margin=" + Quote(Px(cValP(ref Props, "left")) + "," + Px(cValP(ref Props, "top")) + ",0,0");
            S = S + " Width=" + Quote(Px(cValP(ref Props, "width")));
            S = S + " Height=" + Quote(Px(cValP(ref Props, "height")));
            S = S + " VerticalAlignment=" + Quote("Top");
            S = S + " HorizontalAlignment=" + Quote("Left");
        }
        else
        {
            S = "";
            S = S + "<" + tType;
            S = S + " x:Name=\"" + cName + "\"";
            S = S + " Margin=" + Quote(Px(cValP(ref Props, "left")) + "," + Px(cValP(ref Props, "top")) + ",0,0");
            S = S + " Padding=" + Quote("2,2,2,2");
            S = S + " Width=" + Quote(Px(cValP(ref Props, "width")));
            S = S + " Height=" + Quote(Px(cValP(ref Props, "height")));
            S = S + " VerticalAlignment=" + Quote("Top");
            S = S + " HorizontalAlignment=" + Quote("Left");

        }

        if (IsInStr(Features, "Font"))
        {
            S = S + " FontFamily=" + Quote(cValP(ref Props, "font.name", "Calibri"));
            S = S + " FontSize=" + Quote(cValP(ref Props, "font.size", 10));
            if (Val(cValP(ref Props, "font.weight", "400")) > 400)
            {
                S = S + " FontWeight=" + Quote("Bold");
            }

        }

        if (IsInStr(Features, "Content"))
        {
            S = S + " Content=" + QuoteXML(cValP(ref Props, "caption") + cValP(ref Props, "text"));
        }

        if (IsInStr(Features, "Header"))
        {
            S = S + " Content=" + QuoteXML(cValP(ref Props, "caption") + cValP(ref Props, "text"));
        }

        V = cValP(ref Props, "caption") + cValP(ref Props, "text");
        if (IsInStr(Features, "Text") && V != "")
        {
            S = S + " Text=" + QuoteXML(V);
        }

        V = cValP(ref Props, "ToolTipText");
        if (IsInStr(Features, "ToolTip") && V != "")
        {
            S = S + " ToolTip=" + Quote(V);
        }

        S = S + CheckControlEvents(tType, cName, Code);

        if (DoEmpty)
        {
            S = S + " />";
            TagType = "";
        }
        else
        {
            S = S + ">";
            TagType = tType;
        }
        StartControl = S;
        return StartControl;
    }

    public static string CheckControlEvents(string ControlType, string ControlName, string CodeSection = "")
    {
        string CheckControlEvents = "";
        string Res = "";

        bool HasClick = false;
        bool HasFocus = false;
        bool HasChange = false;
        bool IsWindow = false;

        HasClick = true;
        HasFocus = !IsInStr("GroupBox", ControlType);
        HasChange = IsInStr("TextBox,ListBox", ControlType);
        IsWindow = ControlType == "Window";

        Res = "";
        Res = Res + CheckEvent("MouseMove", ControlName, ControlType, CodeSection);
        if (HasFocus)
        {
            Res = Res + CheckEvent("GotFocus", ControlName, ControlType, CodeSection);
            Res = Res + CheckEvent("LostFocus", ControlName, ControlType, CodeSection);
            Res = Res + CheckEvent("KeyDown", ControlName, ControlType, CodeSection);
            Res = Res + CheckEvent("KeyUp", ControlName, ControlType, CodeSection);
        }
        if (HasClick)
        {
            Res = Res + CheckEvent("Click", ControlName, ControlType, CodeSection);
            Res = Res + CheckEvent("DblClick", ControlName, ControlType, CodeSection);
        }
        if (HasChange)
        {
            Res = Res + CheckEvent("Change", ControlName, ControlType, CodeSection);
        }
        if (IsWindow)
        {
            Res = Res + CheckEvent("Load", ControlName, ControlType, CodeSection);
            Res = Res + CheckEvent("Unload", ControlName, ControlType, CodeSection);
            //    Res = Res & CheckEvent("QueryUnload", ControlName, ControlType, CodeSection)
        }

        CheckControlEvents = Res;
        return CheckControlEvents;
    }

    public static string CheckEvent(string EventName, string ControlName, string ControlType, string CodeSection = "")
    {
        string CheckEvent = "";
        string Search = "";
        string Target = "";
        string N = "";

        int L = 0;
        string V = "";

        N = ControlName + "_" + EventName;
        Search = " " + N + "(";
        Target = EventName;
        switch (EventName)
        {
            case "DblClick":
                Target = "MouseDoubleClick";
                break;
            case "Change":
                if (ControlType == "TextBox")
                {
                    Target = "TextChanged";
                }
                break;
            case "Load":
                Target = "Loaded";
                break;
            case "Unload":
                Target = "Unloaded";
                break;
        }
        L = InStr(1, CodeSection, Search, vbTextCompare);
        if (L > 0)
        {
            V = Mid(CodeSection, L + 1, Len(N)); // Get exact capitalization from source....
            CheckEvent = " " + Target + "=\"" + V + "\"";
        }
        else
        {
            CheckEvent = "";
        }
        return CheckEvent;
    }

    public static string EndControl(string tType)
    {
        string EndControl = "";
        switch (tType)
        {
            case "Line":
                EndControl = "";
                break;
            case "Window":
                EndControl = " </Grid>" + vbCrLf + "</Window>";
                break;
            case "GroupBox":
                EndControl = "</Grid> </GroupBox>";
                break;
            default:
                EndControl = "</" + tType + ">";
                break;
        }
        return EndControl;
    }

    public static bool IsEvent(string Str)
    {
        bool IsEvent = false;
        IsEvent = EventStub(Str) != "";
        return IsEvent;
    }

    public static string EventStub(string fName)
    {
        string EventStub = "";
        string S = "";
        string C = "";
        string K = "";


        C = SplitWord(fName, 1, "_");
        K = SplitWord(fName, 2, "_");
        switch (K)
        {
            case "Click":
                S = "private void " + fName + "(object sender, RoutedEventArgs e) { " + fName + "(); }" + vbCrLf;
                break;
            case "Change":
                S = "private void " + C + "_Change(object sender, System.Windows.Controls.TextChangedEventArgs e) { " + fName + "(); }" + vbCrLf;
                break;
            case "QueryUnload":
                S = "private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) { int c = 0, u = 0 ;  " + fName + "(out c, ref u); e.Cancel = c != 0;  }" + vbCrLf;
                //      V = " long doCancel; long UnloadMode; " & FName & "(ref doCancel, ref UnloadMode);"
                break;
            case "Validate":
                //      V = "long doCancel; " & FName & "(ref doCancel);"
                break;
            case "KeyDown":
                break;
            case "MouseMove":
                break;
        }

        EventStub = S;
        return EventStub;
    }
}
