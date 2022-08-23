using UnityEngine.UIElements;

namespace APlus2
{
    public class UIHelper
    {
        public static Label CreateNoMarginPaddingLabel()
        {
            var label = new Label();
            label.style.paddingLeft = 0;
            label.style.paddingRight = 0;
            label.style.paddingBottom = 0;
            label.style.paddingTop = 0;
            label.style.marginTop = 0;
            label.style.marginBottom = 0;
            label.style.marginLeft = 0;
            label.style.marginRight = 0;
            return label;
        }

        public static Icon GetTrueOrFaseMark(bool value)
        {
            return value ? Icons.True : Icons.False;
        }

        public static Icon GetUnsedMark(bool? used)
        {
            if (used.HasValue)
            {
                return GetTrueOrFaseMark(used.Value);
            }

            return Icons.Question;
        }
    }
}