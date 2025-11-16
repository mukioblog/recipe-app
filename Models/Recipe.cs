namespace RecipeApp.Models
{
    public class Recipe
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? ImageBase64 { get; set; }  // 写真
        public List<string> Ingredients { get; set; } = new();  // 材料
        public List<string> Steps { get; set; } = new();  // 手順
        public string? Calories { get; set; }  // カロリー
        public string? CookingTime { get; set; }  // 調理時間
        public string? Servings { get; set; }  // 何人分
        public string? Notes { get; set; }  // メモ・その他
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}