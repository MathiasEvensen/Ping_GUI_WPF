using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace PingGUI.OutputAndInput
{
    public class TextBoxInput : TextReader
        {
            private TextBox textBoxInput = null;
            
            public TextBoxInput(TextBox inputBox)
            {
                textBoxInput = inputBox;
            }
            
            public void Read(char value)
            {
                Thread t = new Thread(Reader);
                t.Start();

                void Reader()
                {
                    base.ReadLine();
                    textBoxInput.Dispatcher.BeginInvoke(
                        new Action(
                            () =>
                            {
                                textBoxInput.GetLineText(value);
                            })
                    );
                }
            }
        }
}