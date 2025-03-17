using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TBAT2CS;

namespace WpfApp1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        _parser = new(t =>
        {
            var isArray = false;
            if (t.Length >= 8 && t[..8] == "Array of")
            {
                isArray = true;
                t = t.Split(' ').Last();
            }
            var transformType = t switch
            {
                "Boolean" => "bool",
                "True" => "bool",
                "String" => "string",
                "Integer" => "long",
                _ => t,
            };
            return isArray ? transformType + "[]" : transformType;
        },
        n =>
        {
            var sb = new StringBuilder();
            foreach (var item in n.Split('_'))
            {
                sb.Append(item.First().ToString().ToUpper());
                sb.Append(item[1..]);
            }

            return sb.ToString();
        });
        InitializeComponent();
        Input.TextChanged += OnTextChanged;
    }

    private Parser _parser;

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Output.Text = _parser.Parse(Input.Text);
    }
}