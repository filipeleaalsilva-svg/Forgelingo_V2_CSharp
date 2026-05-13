using Avalonia.Controls;
using System.Threading.Tasks;

namespace Forgelingo.UI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var save = this.FindControl<Button>("BtnSave");
            save.Click += async (_, __) => await OnSave();
            var box = this.FindControl<TextBox>("ApiKeyBox");
            box.Text = SettingsManager.LoadApiKey();
        }

        private async Task OnSave()
        {
            var box = this.FindControl<TextBox>("ApiKeyBox");
            SettingsManager.SaveApiKey(box.Text ?? "");
            await this.Close();
        }
    }
}
