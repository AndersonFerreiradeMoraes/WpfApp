using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace WpfApp.Models
{
    public class Pedido : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }

        private ObservableCollection<ItemPedido> _produtos = new ObservableCollection<ItemPedido>();
        public ObservableCollection<ItemPedido> Produtos
        {
            get => _produtos;
            set
            {
                if (_produtos != null)
                    _produtos.CollectionChanged -= Produtos_CollectionChanged;

                _produtos = value;

                if (_produtos != null)
                    _produtos.CollectionChanged += Produtos_CollectionChanged;

                OnPropertyChanged(nameof(Produtos));
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        public decimal ValorTotal => Produtos?.Sum(p => p.Quantidade * (p.Produto?.Valor ?? 0)) ?? 0;

        public DateTime DataVenda { get; set; } = DateTime.Now;

        private string _formaPagamento = string.Empty;
        public string FormaPagamento
        {
            get => _formaPagamento;
            set
            {
                if (_formaPagamento != value)
                {
                    _formaPagamento = value;
                    OnPropertyChanged(nameof(FormaPagamento));
                }
            }
        }

        private string _status = "Pendente";
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Produtos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ValorTotal));
        }
    }

    public class ItemPedido : INotifyPropertyChanged
    {
        public int ProdutoId { get; set; }

        private Produto _produto;
        public Produto Produto
        {
            get => _produto;
            set
            {
                if (_produto != value)
                {
                    _produto = value;
                    OnPropertyChanged(nameof(Produto));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        private int _quantidade;
        public int Quantidade
        {
            get => _quantidade;
            set
            {
                if (_quantidade != value)
                {
                    _quantidade = value;
                    OnPropertyChanged(nameof(Quantidade));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal Subtotal => Quantidade * (Produto?.Valor ?? 0);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
