namespace terrain
{
    public class Reservation
    {
        public int Id { get; set; } = 0;
        public DateTime Date { get; set; }
        public DateTime HeureDebut { get; set; }
        public int TerrainId { get; set; } = 0;
        public int UserId { get; set; } = 0;

        public Terrain Terrain { get; set; }
        public User User { get; set; }
    }

}