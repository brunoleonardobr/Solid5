public class Program
{
    static List<Produto> produtos = new();
    static List<Cliente> clientes = new();

    static void Main()
    {
        string opcao;
        IEntradaMenuService entradaMenuService = new EntradaMenuService();
        IProdutoService produtoService = new ProdutoService(produtos);
        IClientesService clientesService = new ClientesService(clientes);
        IPedidoService pedidoService = new PedidoService();
        IReciboService reciboService = new ReciboService();
        IInputService inputService = new ConsoleInputService();
        do
        {
            opcao = entradaMenuService.LerOpcao();
            switch (opcao)
            {
                case "1":
                    produtoService.CadastrarProduto();
                    break;

                case "2":
                    clientesService.CadastrarCliente();
                    break;

                case "3":
                    var cpf = inputService.LerTexto("CPF do cliente: ");
                    var cliente = clientesService.BuscarCliente(cpf);
                    if (cliente == null)
                    {
                        Console.WriteLine("Cliente não encontrado.");
                        break;
                    }

                    var pedido = pedidoService.RealizarPedido(cliente, produtos);
                    var cupom = inputService.LerTexto("Informe o cupom de desconto? ");

                    var desconto = AplicarDescontoFactory.CriarDesconto(cupom);
                    var pedidoComDesconto = desconto.AplicarDesconto(pedido);
                    double total = pedidoService.CalculaTotal(pedidoComDesconto);

                    Console.WriteLine($"\nTotal: R$ {total:F2}");
                    reciboService.SalvarRecibo(pedidoComDesconto, total);
                    break;
            }

        } while (opcao != "4");
    }
}
public interface IInputService
{
    string LerTexto(string prompt);
}
public class ConsoleInputService : IInputService
{
    public string LerTexto(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
interface IEntradaMenuService
{
    string LerOpcao();
}
class EntradaMenuService: IEntradaMenuService
{
    public string LerOpcao()
    {
        Console.Write("Escolha uma opção: ");
        Console.WriteLine("\n1. Cadastrar Produto");
        Console.WriteLine("2. Cadastrar Cliente");
        Console.WriteLine("3. Realizar Pedido");
        Console.WriteLine("4. Sair");
        Console.Write("Opção: ");
        return Console.ReadLine();

    }
}
public interface IProdutoService { void CadastrarProduto(); }
public class ProdutoService(List<Produto> produtos) : IProdutoService
{
    public void CadastrarProduto()
    {
        Console.Write("Nome: ");
        var nome = Console.ReadLine();
        Console.Write("Preço: ");
        var preco = double.Parse(Console.ReadLine());
        produtos.Add(new Produto { Nome = nome, Preco = preco });
        Console.WriteLine("Produto cadastrado!");
    }
}

public interface IClientesService
{
    void CadastrarCliente();
    Cliente BuscarCliente(string cpf);
}
public class ClientesService(List<Cliente> clientes) : IClientesService
{
    public void CadastrarCliente()
    {
        Console.Write("Nome: ");
        var nome = Console.ReadLine();
        Console.Write("CPF: ");
        var cpf = Console.ReadLine();
        clientes.Add(new Cliente { Nome = nome, CPF = cpf });
        Console.WriteLine("Cliente cadastrado!");
    }

    public Cliente BuscarCliente(string cpf) => clientes.Find(c => c.CPF == cpf);
}

public interface IPedidoService
{
    Pedido RealizarPedido(Cliente cliente, List<Produto> produtos);
    double CalculaTotal(Pedido pedido);
}
public class PedidoService : IPedidoService
{
    public Pedido RealizarPedido(Cliente cliente, List<Produto> produtos)
    {
        var pedido = new Pedido { Cliente = cliente, Itens = new List<ItemPedido>() };
        string continuar = "";
        do
        {
            Console.Write("Produto: ");
            var nome = Console.ReadLine();
            var produto = produtos.Find(p => p.Nome == nome);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado.");
                continue;
            }

            Console.Write("Quantidade: ");
            var qtd = int.Parse(Console.ReadLine());
            pedido.Itens.Add(new ItemPedido { Produto = produto, Quantidade = qtd });

            Console.Write("Adicionar mais? (s/n): ");
            continuar = Console.ReadLine();
        } while (continuar == "s");
        return pedido;
    }

    public double CalculaTotal(Pedido pedido) =>
        pedido.Itens.Sum(i => i.Produto.Preco * i.Quantidade);
}

public interface IAplicarDesconto
{
    Pedido AplicarDesconto(Pedido pedido);
}
public class DescontoPorcentagem(double porcentagem) : IAplicarDesconto
{
    public Pedido AplicarDesconto(Pedido pedido)
    {
        foreach (var item in pedido.Itens)
        {
            var precoOriginal = item.Produto.Preco;
            var precoComDesconto = precoOriginal * (1 - porcentagem);
            item.PrecoFinal = precoComDesconto;
        }
        Console.WriteLine($"Desconto de {porcentagem * 100}% aplicado!");
        return pedido;
    }
}
public class SemDesconto : IAplicarDesconto
{
    public Pedido AplicarDesconto(Pedido pedido)
    {
        foreach (var item in pedido.Itens)
            item.PrecoFinal = item.Produto.Preco;

        Console.WriteLine("Nenhum desconto aplicado.");
        return pedido;
    }
}

public static class AplicarDescontoFactory
{
    public static IAplicarDesconto CriarDesconto(string cupom) => cupom switch
    {
        "10" => new DescontoPorcentagem(0.10),
        "20" => new DescontoPorcentagem(0.20),
        _ => new SemDesconto()
    };
}

public interface IReciboService
{
    void SalvarRecibo(Pedido pedido, double total);
}
public class ReciboService : IReciboService
{
    public void SalvarRecibo(Pedido pedido, double total)
    {
        Console.WriteLine("\nRecibo:");
        Console.WriteLine($"Cliente: {pedido.Cliente.Nome} - CPF: {pedido.Cliente.CPF}");
        foreach (var item in pedido.Itens)
        {
            Console.WriteLine($"{item.Produto.Nome} x{item.Quantidade} - R$ {item.PrecoFinal * item.Quantidade:F2}");
        }
        Console.WriteLine($"Total: R$ {total:F2}");
        File.WriteAllText("recibo.txt", $"Recibo gerado para {pedido.Cliente.Nome} - Total: R$ {total:F2}");
        Console.WriteLine("Recibo salvo em recibo.txt");
    }
}


public class Produto
{
    public string Nome { get; set; }
    public double Preco { get; set; }
}

public class Cliente
{
    public string Nome { get; set; }
    public string CPF { get; set; }
}

public class Pedido
{
    public Cliente Cliente { get; set; }
    public List<ItemPedido> Itens { get; set; }
}

public class ItemPedido
{
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public double PrecoFinal { get; set; }
}
