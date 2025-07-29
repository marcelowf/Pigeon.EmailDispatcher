using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pigeon.EmailDispatcher.Service.Models
{
    public class AttachmentModel
    {
        public required string FileName { get; set; }
        public required string ContentBase64 { get; set; }
        public required string ContentType { get; set; }
    }
}
