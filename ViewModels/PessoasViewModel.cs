using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels
{
    public class PessoasViewModel : ViewModelBase
    {
        private readonly JsonDataService _data;

        private ObservableCollection<Pessoa> _pessoas;
        public ObservableCollection<Pessoa> Pessoas
        {
            get => _pessoas;
            set { _pessoas = value; OnPropertyChanged(); }
        }

        private string _filtroNome;
        public string FiltroNome
        {
            get => _filtroNome;
            set { _filtroNome = value; OnPropertyChanged(); Filtrar(); }
        }

        private string _filtroCpf;
        public string FiltroCpf
        {
            get => _filtroCpf;
            set { _filtroCpf = value; OnPropertyChanged(); Filtrar(); }
        }

        public ICommand SalvarCommand { get; }

        public PessoasViewModel()
        {
            _data = new JsonDataService("Data/pessoas.json");
            Pessoas = new ObservableCollection<Pessoa>(_data.LoadAll<Pessoa>());
            SalvarCommand = new RelayCommand(_ => Salvar());
        }

        // Filtra a lista de pessoas com base nos filtros
        private void Filtrar()
        {
            var all = _data.LoadAll<Pessoa>()
                .Where(p => string.IsNullOrEmpty(FiltroNome) || p.Nome.ToLower().Contains(FiltroNome.ToLower()))
                .Where(p => string.IsNullOrEmpty(FiltroCpf) || p.CPF.Contains(FiltroCpf))
                .ToList();

            Pessoas.Clear();
            foreach (var p in all) Pessoas.Add(p);
        }

        // Adiciona uma nova pessoa na coleção e salva no JSON
        public void Incluir(Pessoa p)
        {
            var list = _data.LoadAll<Pessoa>();
            p.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            list.Add(p);
            _data.SaveAll(list);
            Pessoas.Add(p);
        }

        // Salvar chamado pelo botão Salvar
        public void Salvar()
        {
            // Cria nova pessoa a partir dos campos do formulário
            var novaPessoa = new Pessoa
            {
                Nome = FiltroNome,
                CPF = FiltroCpf,
                Endereco = "" // você pode adicionar campo Endereco se quiser
            };

            // Inclui e salva
            Incluir(novaPessoa);

            // Limpa campos
            FiltroNome = string.Empty;
            FiltroCpf = string.Empty;
        }

        // Exclui pessoa selecionada
        public void Excluir(Pessoa p)
        {
            if (p == null) return;

            var list = _data.LoadAll<Pessoa>();
            list.RemoveAll(x => x.Id == p.Id);
            _data.SaveAll(list);
            Pessoas.Remove(p);
        }
    }
}
