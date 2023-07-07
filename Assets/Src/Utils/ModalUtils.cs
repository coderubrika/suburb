using Suburb.UI.Layouts;
using System.Collections.Generic;
using System.Linq;

namespace Suburb.Utils
{
    public static class ModalUtils
    {
        public static readonly IEnumerable<(string, string)> HaveSaveChangesInput = HaveSaveChanges
            .Append(Yes);

        public static readonly IEnumerable<(string, string)> HaveSaveChangesCancelInput = HaveSaveChanges
            .Union(YesNo);

        public static readonly IEnumerable<(string, string)> AskRewriteSaveInput = AskRewriteSave
            .Append(Yes);

        private static readonly (string, string) Yes = (ModalConfirmLayout.CONFIRM_LABEL, "Да");

        private static readonly (string, string) No = (ModalConfirmLayout.CONFIRM_LABEL, "Нет");

        private static readonly (string, string)[] YesNo = new (string, string)[]
        {
            Yes, No
        };

        private static readonly IEnumerable<(string, string)> HaveSaveChanges = new (string, string)[]
        {
            (ModalConfirmLayout.HEADER_LABEL, "Есть несохраненные изменения"),
            (ModalConfirmLayout.BODY_LABEL, "Хотите сохранить изменения?"),
        };

        private static readonly IEnumerable<(string, string)> AskRewriteSave = new (string, string)[]
        {
            (ModalConfirmLayout.HEADER_LABEL, "Перезапись сохранения"),
            (ModalConfirmLayout.BODY_LABEL, "Уверены, что хотите перезаписать это сохранение?"),
        };

    }
}
