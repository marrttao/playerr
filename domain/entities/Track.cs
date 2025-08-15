namespace playerr.domain.entities
{
    public class Track
    {
        public string Id { get; set; }           
        public string Name { get; set; }         
        public string Artist { get; set; }       
        public string Source { get; set; }        
        public bool IsLocal { get; set; } 
    }

}