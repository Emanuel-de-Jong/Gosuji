namespace Gosuji.Client.Data
{
    public interface IDbModel
    {
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset ModifyDate { get; set; }
    }
}
