using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Deployer.Ide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompletionWindow completionWindow;

        public MainWindow()
        {
            InitializeComponent();
            //completionWindow = new CompletionWindow(this.TextEditor.TextArea);
            //this.TextEditor.TextArea.TextEntering += TextAreaOnTextEntering;
            //this.TextEditor.TextArea.KeyDown += TextAreaOnKeyDown;
        }

        private void TextAreaOnKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Space && Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    // Open code completion after the user has pressed dot:
            //    completionWindow = new CompletionWindow(TextEditor.TextArea);
            //    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            //    data.Add(new MyCompletionData("GitHub"));
            //    data.Add(new MyCompletionData("Item2"));
            //    data.Add(new MyCompletionData("Item3"));
            //    completionWindow.Show();
            //    completionWindow.Closed += delegate
            //    {
            //        completionWindow = null;
            //    };

            //    e.Handled = true;
            //}
        }

        private void TextAreaOnTextEntering(object sender, TextCompositionEventArgs e)
        {
            //if (e.Text.Length > 0 && completionWindow != null)
            //{
            //    if (!char.IsLetterOrDigit(e.Text[0]))
            //    {
            //        // Whenever a non-letter is typed while the completion window is open,
            //        // insert the currently selected element.
            //        completionWindow.CompletionList.RequestInsertion(e);
            //    }
            //}
        }
    }

    //public class MyCompletionData : ICompletionData
    //{
    //    public MyCompletionData(string text)
    //    {
    //        this.Text = text;
    //    }

    //    public System.Windows.Media.ImageSource Image
    //    {
    //        get { return null; }
    //    }

    //    public string Text { get; private set; }

    //    // Use this property if you want to show a fancy UIElement in the list.
    //    public object Content
    //    {
    //        get { return this.Text; }
    //    }

    //    public object Description
    //    {
    //        get { return "Description for " + this.Text; }
    //    }

    //    public double Priority => 1;

    //    public void Complete(TextArea textArea, ISegment completionSegment,
    //        EventArgs insertionRequestEventArgs)
    //    {
    //        textArea.Document.Replace(completionSegment, this.Text);
    //    }
    //}
}
