class OrderTable
{
    public OrderTable(double price, double coin1, double coin2)
    {
        this.price = price.ToString("F8");
        this.coin1 = coin1.ToString("F8");
        this.coin2 = coin2.ToString("F8");
    }
    public string price { get; set; }
    public string coin1 { get; set; }
    public string coin2 { get; set; }
}