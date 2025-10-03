using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp.ViewModels;
using WpfApp.Models;

namespace WpfApp.Views
{
    public partial class PedidosView : UserControl
    {
        private PedidosViewModel vm;

        public PedidosView()
        {
            InitializeComponent();
            vm = new PedidosViewModel();
            DataContext = vm;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm.CarregarPedidos();
            vm.CarregarPessoas();
            vm.CarregarProdutos();
        }

        #region Botões do Formulário

        private void CriarPedido_Click(object sender, RoutedEventArgs e)
        {
            vm.CriarPedido();
            MessageBox.Show("Pedido criado. Agora adicione produtos e finalize.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AdicionarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (vm.PedidoAtual == null)
            {
                MessageBox.Show("Crie um pedido antes de adicionar produtos.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (vm.SelectedProduto == null)
            {
                MessageBox.Show("Selecione um produto antes de adicionar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            vm.AddProdutoToPedido();
        }

        private void FinalizarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (vm.PedidoAtual == null)
            {
                MessageBox.Show("Nenhum pedido em edição para finalizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            vm.FinalizarPedido();
            MessageBox.Show("Pedido finalizado e salvo.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Botões de Filtro

        private void MostrarTodos_Click(object sender, RoutedEventArgs e) => vm.CarregarPedidos();

        private void MostrarPendentes_Click(object sender, RoutedEventArgs e)
        {
            var pendentes = vm.Pedidos.Where(p => p.Status == "Pendente").ToList();
            vm.Pedidos.Clear();
            foreach (var p in pendentes) vm.Pedidos.Add(p);
        }

        private void MostrarPagos_Click(object sender, RoutedEventArgs e)
        {
            var pagos = vm.Pedidos.Where(p => p.Status == "Pago").ToList();
            vm.Pedidos.Clear();
            foreach (var p in pagos) vm.Pedidos.Add(p);
        }

        #endregion

        #region Botões de Status

        private void MarcarPago_Click(object sender, RoutedEventArgs e)
        {
            if (dgPedidos.SelectedItem is Pedido pedido)
                vm.AtualizarStatus(pedido, "Pago");
        }

        private void MarcarEnviado_Click(object sender, RoutedEventArgs e)
        {
            if (dgPedidos.SelectedItem is Pedido pedido)
                vm.AtualizarStatus(pedido, "Enviado");
        }

        private void MarcarRecebido_Click(object sender, RoutedEventArgs e)
        {
            if (dgPedidos.SelectedItem is Pedido pedido)
                vm.AtualizarStatus(pedido, "Recebido");
        }

        #endregion
    }
}
