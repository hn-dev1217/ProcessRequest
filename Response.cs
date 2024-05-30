using Microsoft.Identity.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRequest
{
    public class Response
    {
        public string id
        {
            get; set;
        }
        public string starting
        {
            get; set;
        }

        public string ending
        {
            get; set;
        }

        public string shortestPath
        {
            get; set;
        }

        public int numberOfMoves
        {
            get; set;
        }

        public string operationId
        {
            get; set;
        }
    }
}
