using Suburb.UI.Layouts;
using System.Collections.Generic;
using System.Linq;

namespace Suburb.Utils
{
    public static class ModalUtils
    {
        private static readonly IEnumerable<(string, string)> HaveSaveChanges = new (string, string)[]
        {
            (ModalConfirmLayout.HEADER_LABEL, "modal_header_unsave_changes"),
            (ModalConfirmLayout.BODY_LABEL, "modal_body_unsave_changes"),
        };

        private static readonly IEnumerable<(string, string)> AskRewriteSave = new (string, string)[]
        {
            (ModalConfirmLayout.HEADER_LABEL, "modal_header_rewrite_save"),
            (ModalConfirmLayout.BODY_LABEL, "modal_body_rewrite_save"),
        };

        private static readonly IEnumerable<(string, string)> AskDeleteSave = new (string, string)[]
        {
            (ModalConfirmLayout.HEADER_LABEL, "modal_header_delete_save"),
            (ModalConfirmLayout.BODY_LABEL, "modal_body_delete_save"),
        };

        private static readonly (string, string) Yes = (ModalConfirmLayout.CONFIRM_LABEL, "yes");

        private static readonly (string, string) No = (ModalConfirmCancelLayout.CANCEL_LABEL, "no");

        private static readonly (string, string)[] YesNo = new (string, string)[]
        {
            Yes, No
        };

        public static readonly IEnumerable<(string, string)> HaveSaveChangesInput = HaveSaveChanges
            .Append(Yes);

        public static readonly IEnumerable<(string, string)> HaveSaveChangesCancelInput = HaveSaveChanges
            .Union(YesNo);

        public static readonly IEnumerable<(string, string)> AskRewriteSaveInput = AskRewriteSave
            .Append(Yes);

        public static readonly IEnumerable<(string, string)> AskDeleteSaveInput = AskDeleteSave
            .Append(Yes);
    }
}
