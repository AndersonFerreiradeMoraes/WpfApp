using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class ProdutosViewModel : ViewModelBase
    {
        private readonly JsonDataService _data;

        private ObservableCollection<Produto> _produtos;
        public ObservableCollection<Produto> Produtos
        {
            get => _produtos;
            set { _produtos = value; OnPropertyChanged(); }
        }

        private string _filtroNome;
        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); Filtrar(); }
        }

        private string _filtroCodigo;
        public string FiltroCodigo
        {
            get => _filtroCodigo;
            set { _filtroCodigo = value; OnPropertyChanged(); Filtrar(); }
        }

        private decimal? _valorMin;
        public decimal? ValorMin
        {
            get => _valorMin;
            set { _valorMin = value; OnPropertyChanged(); Filtrar(); }
        }

        private decimal? _valorMax;
        public decimal? ValorMax
        {
            get => _valorMax;
            set { _valorMax = value; OnPropertyChanged(); Filtrar(); }
        }

        public ICommand SalvarCommand { get; }

        public ProdutosViewModel()
        {
            _data = new JsonDataService("Data/produtos.json");
            Produtos = new ObservableCollection<Produto>(_data.LoadAll<Produto>());
            SalvarCommand = new RelayCommand(_ => Salvar());
        }

        // Filtra produtos com base nos campos
        private void Filtrar()
        {
            var all = _data.LoadAll<Produto>()
                .Where(p => string.IsNullOrEmpty(FiltroNome) || p.Nome.ToLower().Contains(FiltroNome.ToLower()))
                .Where(p => string.IsNullOrEmpty(FiltroCodigo) || p.Codigo.ToLower().Contains(FiltroCodigo.ToLower()))
                .Where(p => !ValorMin.HasValue || p.Valor >= ValorMin.Value)
                .Where(p => !ValorMax.HasValue || p.Valor <= ValorMax.Value)
                .ToList();

            Produtos.Clear();
            foreach (var p in all) Produtos.Add(p);
        }

        // Inclui e salva um novo produto
        public void Salvar()
        {
            if (string.IsNullOrWhiteSpace(FiltroNome) || string.IsNullOrWhiteSpace(FiltroCodigo))
                return;

            var list = _data.LoadAll<Produto>();

            var novoProduto = new Produto
            {
                Id = list.Any() ? list.Max(x => x.Id) + 1 : 1,
                Nome = FiltroNome,
                Codigo = FiltroCodigo,
                Valor = ValorMin ?? 0
            };

            list.Add(novoProduto);
            Produtos.Add(novoProduto);
            _data.SaveAll(list);

            // Limpa campos
            FiltroNome = string.Empty;
            FiltroCodigo = string.Empty;
            ValorMin = null;
            ValorMax = null;
        }

        // Excluir produto selecionado
        public void Excluir(Produto p)
        {
            if (p == null) return;

            var list = _data.LoadAll<Produto>();
            list.RemoveAll(x => x.Id == p.Id);
            _data.SaveAll(list);
            Produtos.Remove(p);
        }
    }
}
