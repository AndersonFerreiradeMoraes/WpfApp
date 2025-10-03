using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class PedidosViewModel : ViewModelBase
    {
        private readonly JsonDataService _dataPedidos;
        private readonly JsonDataService _dataPessoas;
        private readonly JsonDataService _dataProdutos;
        private FileSystemWatcher _watcherPedidos;
        private FileSystemWatcher _watcherPessoas;
        private FileSystemWatcher _watcherProdutos;
        private bool _atualizando = false;

        public ObservableCollection<Pedido> Pedidos { get; set; } = new ObservableCollection<Pedido>();
        public ObservableCollection<Pessoa> Pessoas { get; set; } = new ObservableCollection<Pessoa>();
        public ObservableCollection<Produto> Produtos { get; set; } = new ObservableCollection<Produto>();

        private Pessoa _selectedPessoa;
        public Pessoa SelectedPessoa
        {
            get => _selectedPessoa;
            set { _selectedPessoa = value; OnPropertyChanged(nameof(SelectedPessoa)); }
        }

        private Produto _selectedProduto;
        public Produto SelectedProduto
        {
            get => _selectedProduto;
            set { _selectedProduto = value; OnPropertyChanged(nameof(SelectedProduto)); }
        }

        private int _quantidade = 1;
        public int Quantidade
        {
            get => _quantidade;
            set { _quantidade = value; OnPropertyChanged(nameof(Quantidade)); }
        }

        private string _formaPagamento;
        public string FormaPagamento
        {
            get => _formaPagamento;
            set { _formaPagamento = value; OnPropertyChanged(nameof(FormaPagamento)); }
        }

        // Pedido em edição
        private Pedido _pedidoAtual;
        public Pedido PedidoAtual
        {
            get => _pedidoAtual;
            set { _pedidoAtual = value; OnPropertyChanged(nameof(PedidoAtual)); }
        }

        public PedidosViewModel()
        {
            _dataPedidos = new JsonDataService("Data/pedidos.json");
            _dataPessoas = new JsonDataService("Data/pessoas.json");
            _dataProdutos = new JsonDataService("Data/produtos.json");

            // Carrega inicialmente
            CarregarPedidos();
            CarregarPessoas();
            CarregarProdutos();

            // Configura watchers
            ConfigurarWatcher();
        }

        #region Carregar Dados

        public void CarregarPedidos()
        {
            var list = _dataPedidos.LoadAll<Pedido>() ?? new System.Collections.Generic.List<Pedido>();
            App.Current.Dispatcher.Invoke(() =>
            {
                Pedidos.Clear();
                foreach (var pedido in list)
                    Pedidos.Add(pedido);
            });
        }

        public void CarregarPessoas()
        {
            var list = _dataPessoas.LoadAll<Pessoa>() ?? new System.Collections.Generic.List<Pessoa>();
            App.Current.Dispatcher.Invoke(() =>
            {
                Pessoas.Clear();
                foreach (var p in list)
                    Pessoas.Add(p);
            });
        }

        public void CarregarProdutos()
        {
            var list = _dataProdutos.LoadAll<Produto>() ?? new System.Collections.Generic.List<Produto>();
            App.Current.Dispatcher.Invoke(() =>
            {
                Produtos.Clear();
                foreach (var p in list)
                    Produtos.Add(p);
            });
        }

        #endregion

        #region FileSystemWatcher

        private void ConfigurarWatcher()
        {
            _watcherPedidos = CriarWatcher("Data", "pedidos.json", CarregarPedidos);
            _watcherPessoas = CriarWatcher("Data", "pessoas.json", CarregarPessoas);
            _watcherProdutos = CriarWatcher("Data", "produtos.json", CarregarProdutos);
        }

        private FileSystemWatcher CriarWatcher(string path, string fileName, Action acao)
        {
            var watcher = new FileSystemWatcher(path, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            watcher.Changed += (s, e) =>
            {
                if (_atualizando) return;
                _atualizando = true;
                Thread.Sleep(120);
                acao();
                _atualizando = false;
            };
            return watcher;
        }

        #endregion

        #region Fluxo de Pedido (novo padrão com PedidoAtual)

        // Cria um novo pedido e o define como PedidoAtual (aparece no DataGrid)
        public void CriarPedido()
        {
            var list = _dataPedidos.LoadAll<Pedido>() ?? new System.Collections.Generic.List<Pedido>();
            var p = new Pedido
            {
                Id = list.Any() ? list.Max(x => x.Id) + 1 : 1,
                PessoaId = SelectedPessoa?.Id ?? 0,
                PessoaNome = SelectedPessoa?.Nome ?? string.Empty,
                DataVenda = DateTime.Now,
                FormaPagamento = FormaPagamento ?? string.Empty,
                Status = "Pendente",
                Produtos = new ObservableCollection<ItemPedido>()
            };

            PedidoAtual = p;
            App.Current.Dispatcher.Invoke(() => Pedidos.Add(p));
        }

        // Adiciona o produto selecionado ao PedidoAtual
        public void AddProdutoToPedido()
        {
            if (PedidoAtual == null) return;
            if (SelectedProduto == null || Quantidade <= 0) return;

            var item = new ItemPedido
            {
                ProdutoId = SelectedProduto.Id,
                Produto = SelectedProduto,
                Quantidade = Quantidade
            };

            if (PedidoAtual.Produtos == null)
                PedidoAtual.Produtos = new ObservableCollection<ItemPedido>();

            PedidoAtual.Produtos.Add(item);

            // Notifica atualização do valor total
            OnPropertyChanged(nameof(PedidoAtual));
        }

        // Salva todos os pedidos e limpa o PedidoAtual
        public void FinalizarPedido()
        {
            if (PedidoAtual == null) return;

            // salva todos os pedidos atuais no arquivo (inclui o pedido que já foi adicionado à coleção)
            _dataPedidos.SaveAll(Pedidos.ToList());

            // limpa estado de edição
            PedidoAtual = null;
            SelectedPessoa = null;
            SelectedProduto = null;
            Quantidade = 1;
            FormaPagamento = string.Empty;
        }

        public void SalvarPedidos()
        {
            _dataPedidos.SaveAll(Pedidos.ToList());
        }

        public void AtualizarStatus(Pedido pedido, string status)
        {
            if (pedido == null) return;
            pedido.Status = status;
            SalvarPedidos();
            OnPropertyChanged(nameof(Pedidos));
        }

        #endregion
    }
}
