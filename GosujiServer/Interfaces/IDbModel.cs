namespace GosujiServer.Interfaces
{
    public interface IDbModel
    {
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset? ModifyDate { get; set; }
    }
}
