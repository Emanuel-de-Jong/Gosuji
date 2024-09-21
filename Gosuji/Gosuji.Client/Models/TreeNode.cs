namespace Gosuji.Client.Models
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; set; } = [];
    }
}
