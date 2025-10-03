using System.Windows;
using System.Windows.Controls;
using WpfApp.Models;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class ProdutosView : UserControl
    {
        public ProdutosView()
        {
            InitializeComponent();
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProdutosViewModel vm) vm.Salvar();
            MessageBox.Show("Produtos salvos.");
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProdutosViewModel vm && (this.Content as Grid).Children[1] is DataGrid dg && dg.SelectedItem is Produto p)
            {
                if (MessageBox.Show($"Excluir {p.Nome}?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    vm.Excluir(p);
                }
            }
        }
    }
}
