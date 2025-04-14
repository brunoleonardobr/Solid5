public class Program
{
    static List<Produto> produtos = new();
    static List<Cliente> clientes = new();

    static void Main()
    {
        string opcao;
        do
        {

            Console.WriteLine("\n1. Cadastrar Produto");
            Console.WriteLine("2. Cadastrar Cliente");
            Console.WriteLine("3. Realizar Pedido");
            Console.WriteLine("4. Sair");
            Console.Write("Opção: ");
            opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Nome: ");
                    var nome = Console.ReadLine();
                    Console.Write("Preço: ");
                    var preco = double.Parse(Console.ReadLine());
                    produtos.Add(new Produto { Nome = nome, Preco = preco });
                    Console.WriteLine("Produto cadastrado!");
                    break;

                case "2":
                    Console.Write("Nome do cliente: ");
                    var nomeCliente = Console.ReadLine();
                    Console.Write("CPF: ");
                    var cpf = Console.ReadLine();
                    clientes.Add(new Cliente { Nome = nomeCliente, CPF = cpf });
                    Console.WriteLine("Cliente cadastrado!");
                    break;

                case "3":
                    Console.Write("CPF do cliente: ");
                    var cpfBusca = Console.ReadLine();
                    var cliente = clientes.Find(c => c.CPF == cpfBusca);
                    if (cliente == null)
                    {
                        Console.WriteLine("Cliente não encontrado.");
                        break;
                    }

                    var pedido = new Pedido { Cliente = cliente, Itens = new List<ItemPedido>() };
                    string continuar = "";
                    do
                    {
                        Console.Write("Nome do produto: ");
                        var prodNome = Console.ReadLine();
                        var produto = produtos.Find(p => p.Nome == prodNome);
                        if (produto == null)
                        {
                            Console.WriteLine("Produto não encontrado.");
                            continue;
                        }

                        Console.Write("Quantidade: ");
                        int qtd = int.Parse(Console.ReadLine());
                        pedido.Itens.Add(new ItemPedido { Produto = produto, Quantidade = qtd });

                        Console.Write("Adicionar mais? (s/n): ");
                        continuar = Console.ReadLine();
                    } while (continuar == "s");
                    Console.Write("Cupom de desconto (PROMO10)? ");
                    var cupom = Console.ReadLine();
                    double total = 0;
                    foreach (var item in pedido.Itens)
                        total += item.Produto.Preco * item.Quantidade;
                    if (cupom == "PROMO10")
                    {
                        total *= 0.9;
                        Console.WriteLine("Desconto de 10% aplicado!");
                    }
                    Console.WriteLine($"\nTotal: R$ {total:F2}");
                    Console.WriteLine("Recibo:");
                    Console.WriteLine($"Cliente: {pedido.Cliente.Nome} - {pedido.Cliente.CPF}");
                    foreach (var item in pedido.Itens)
                        Console.WriteLine($"{item.Produto.Nome} x{item.Quantidade} - R$ {item.Produto.Preco * item.Quantidade}");

                    File.WriteAllText("recibo.txt", $"Recibo de {pedido.Cliente.Nome} - Total: {total:F2}");
                    Console.WriteLine("Recibo salvo em recibo.txt");
                    break;
            }

        } while (opcao != "4");
    }
}

class Produto
{
    public string Nome { get; set; }
    public double Preco { get; set; }
}

class Cliente
{
    public string Nome { get; set; }
    public string CPF { get; set; }
}

class Pedido
{
    public Cliente Cliente { get; set; }
    public List<ItemPedido> Itens { get; set; }
}

class ItemPedido
{
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
}
