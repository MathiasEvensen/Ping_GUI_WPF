using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace PingGUI.OutputAndInput
{
    public class TextBoxOutput : TextWriter
    {

        private TextBox textBox = null;

        public TextBoxOutput(TextBox output)
        {
            textBox = output;
        }

        public override void Write(char value)
        {
            Thread t = new Thread(Writer);
            t.Start();

            void Writer()
            {
                base.Write(value);
                textBox.Dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            textBox.AppendText(value.ToString());
                        })
                    );
            }
        }

        public void CLS()
        {
            textBox.Text = String.Empty;
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}