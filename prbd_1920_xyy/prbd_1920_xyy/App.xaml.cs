using PRBD_Framework;
using prbd_1920_xyy.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace prbd_1920_xyy {
    public enum AppMessages {
        MSG_OPEN_INDEX,

        MSG_OPEN_QUESTION,
        MSG_CLOSE_QUESTION,

        MSG_OPEN_EDIT,
        MSG_CLOSE_EDIT,

        MSG_OPEN_DELETE,
        MSG_CLOSE_DELETE,

        MSG_OPEN_ACCEPT,
        MSG_CLOSE_ACCEPT,

        MSG_OPEN_COMMENT,
        MSG_CLOSE_COMMENT,

        MSG_OPEN_EDIT_COMMENT,
        MSG_CLOSE_EDIT_COMMENT,

        MSG_OPEN_VOTE,
        MSG_CLOSE_VOTE,

        MSG_CLOSE_ASK,

        MSG_POST_CHANGED,
        MSG_VOTE_CHANGED,
        MSG_COMMENT_CHANGED,

        MSG_TAG_ADDED,
        MSG_ANSWEAR_ADDED,
        MSG_COMMENT_ADDED,

        MSG_POST_DELETED,
        MSG_ANSWEAR_DELETED,
        MSG_TAG_DELETED,

        MSG_ANSWEAR_ACCEPTED,
        MSG_SORT_BY_TAG,
    }

    public partial class App : ApplicationBase {
        public static Model Model { get; private set; } = new Model();

        public static readonly string IMAGE_PATH =
            Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../images");
        public static User CurrentUser { get; set; }

        public static void CancelChanges() {
            Model.Dispose(); // Retire de la mémoire le modèle actuel
            Model = new Model(); // Recréation d'un nouveau modèle à partir de la DB
        }
        public App() {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Culture);
            ColdStart();
        }

        private void ColdStart() {
            Model.SeedData();
        }
    }
}
