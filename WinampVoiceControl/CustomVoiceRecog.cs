using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

public class CustomVoiceRecog
{
    private delegate void SpeechEventHandler(object sender, string recognized);
    private event SpeechEventHandler SpeechEvent;
    private float RequiredConfidence = 0.92f;
    private List<String> commands = new List<String>();
    private SpeechRecognitionEngine rec;
    private bool grammarLoaded = false;
    private bool listening = true;

    public IntPtr winampHandle;

    // Winamp Constants
    const int WA_NOTHING = 0;
    const int WA_PREVTRACK = 40044;
    const int WA_PLAY = 40045;
    const int WA_PAUSE = 40046;
    const int WA_STOP = 40047;
    const int WA_NEXTTRACK = 40048;
    const int WA_VOLUMEUP = 40058;
    const int WA_VOLUMEDOWN = 40059;
    const int WINAMP_FFWD5S = 40060;
    const int WINAMP_REW5S = 40061;
    const int WM_COMMAND = 0x111;


    public void InitVoiceHandler() // Do this after loading word choices, or else reload the GrammarBuilder
    {
        rec = new SpeechRecognitionEngine();
        rec.SetInputToDefaultAudioDevice();

        if (commands.Count > 0)
        {
            Choices c = new Choices();

            foreach (string i in commands)
            {
                c.Add(i);
            }


            GrammarBuilder gb = new GrammarBuilder(c);
            Grammar g = new Grammar(gb);
            rec.LoadGrammar(g);
            grammarLoaded = true;
        }
        else
        {
            Console.WriteLine("ERROR: Need at least one command in wordlist.");
        }
    }

    public void StartListening()
    {
        if (grammarLoaded)
        {
            rec.SpeechRecognized += rec_SpeechRecognized;
            rec.RecognizeAsync(RecognizeMode.Multiple);
        }
    }
    public void StopListening()
    {
        rec.RecognizeAsyncStop();
        rec.SpeechRecognized -= rec_SpeechRecognized;
    }

    public void AddCommandWord(String command)
    {
        commands.Add(command);
    }

    public void RemoveCommandWord(String word)
    {
        if (commands.Contains(word))
            commands.Remove(word);
    }

    public void winampListen(byte state, byte beep)
    {
        if (state == 0) // Stop listening
        {
            listening = false;
            if (beep == 1)
                Console.Beep(200, 200);
        }
        else if (state == 1) // Start Listening
        {
            listening = true;
            if (beep == 1)
                Console.Beep(1000, 200);
        }
    }

    // Privates
    private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {

        if (e.Result.Confidence >= RequiredConfidence)
        {
            String text = e.Result.Text;
            Console.WriteLine(text);
            if (listening)
            {
                if (text == "pause")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WA_PAUSE, WA_NOTHING);
                else if (text == "play")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WA_PLAY, WA_NOTHING);
                else if (text == "next")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WA_NEXTTRACK, WA_NOTHING);
                else if (text == "previous" || text == "last")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WA_PREVTRACK, WA_NOTHING);
                else if (text == "stop")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WA_STOP, WA_NOTHING);
                else if (text == "volume up ten")
                    volumeControl(5, WA_VOLUMEUP); // 5 = 10%
                else if (text == "volume down ten")
                    volumeControl(5, WA_VOLUMEDOWN);
                else if (text == "volume up fifty")
                    volumeControl(30, WA_VOLUMEUP);
                else if (text == "volume down fifty")
                    volumeControl(30, WA_VOLUMEDOWN);
                else if (text == "volume up max")
                    volumeControl(100, WA_VOLUMEUP);
                else if (text == "volume down max")
                    volumeControl(100, WA_VOLUMEDOWN);
                else if (text == "forward")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WINAMP_FFWD5S, WA_NOTHING);
                else if (text == "rewind")
                    SendMessage.SendMessageA(winampHandle, WM_COMMAND, WINAMP_REW5S, WA_NOTHING);
            }

            // Control
            if (text == "winamp listen")
                winampListen(1, 1);
            else if (text == "winamp ignore")
                winampListen(0, 1);


        }
        
    }



    private void volumeControl(int amount, int command) // One command = 2%
    {
        for (int i = 0; i < amount+1; ++i)
        {
            SendMessage.SendMessageA(winampHandle, WM_COMMAND, command, WA_NOTHING);
            Thread.Sleep(25);
        }
    }
    private void RaiseSpeechEvent(string recognized)
    {
        if (SpeechEvent != null)
            SpeechEvent(null, recognized);
    }
}
