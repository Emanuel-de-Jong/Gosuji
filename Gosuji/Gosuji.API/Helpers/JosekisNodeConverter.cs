using Gosuji.Client.Models.Josekis;
using IGOEnchi.GoGameLogic;

namespace Gosuji.API.Helpers
{
    public class JosekisNodeConverter
    {
        public static JosekisNode Convert(GoNode node)
        {
            return new JosekisNode(node.Comment, ToJosekisLabels(node.Markup.Labels), ToJosekisMarks(node.Markup.Marks));
        }

        public static JosekisNode Convert(GoMoveNode node)
        {
            return new JosekisNode(MoveHelper.FromIGOEnchi(node.Stone), node.Comment,
                ToJosekisLabels(node.Markup.Labels), ToJosekisMarks(node.Markup.Marks));
        }

        private static List<JosekisLabel> ToJosekisLabels(List<TextLabel> labels)
        {
            return labels.Select(l => new JosekisLabel(l.X, l.Y, l.Text)).ToList();
        }

        private static List<JosekisMark> ToJosekisMarks(List<Mark> marks)
        {
            return marks.Select(m => new JosekisMark(m.X, m.Y, (JosekisMarkType)(int)m.MarkType)).ToList();
        }
    }
}
