using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class PessoasView : UserControl
    {
        public PessoasView()
        {
            InitializeComponent();
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as PessoasViewModel;
            if (vm == null) return;
            if ((this.Content as Grid).Children[1] is DataGrid dg && dg.SelectedItem is Pessoa p)
            {
                if (MessageBox.Show($"Excluir {p.Nome}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    vm.Excluir(p);
                }
            }
        }
    }
}
