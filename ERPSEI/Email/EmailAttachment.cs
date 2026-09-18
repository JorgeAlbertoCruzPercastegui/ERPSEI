namespace ERPSEI.Email
{
    public class EmailAttachment
    {
        public string FileName
        {
            get;
            set;
        } = string.Empty;


        public string ContentType
        {
            get;
            set;
        } = "application/octet-stream";


        public byte[] ContentBytes
        {
            get;
            set;
        } = Array.Empty<byte>();
    }
}