using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace LibMessage
{
    [Serializable]
    public class ChatMessage
    {
        public List<string> ClientList = new List<string>();       // the user's list of the cuurently online clients
        public string UserName { get; set; }        // the suer's username
        public string Msg { get; set; }           // the message the user has sent
        public Color color { get; set; }         // the text color which the user has chosen  
        public Image UploadedImage { get; set; }    // the image the user has recently uploaded
        public int UserID { get; set; }          // the user's user ID which is recieved from the database, it is NOT mentioned in the program's UI and the user is unaware of it
    }
}
