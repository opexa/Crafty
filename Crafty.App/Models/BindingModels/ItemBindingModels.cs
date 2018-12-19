namespace Crafty.App.Models.BindingModels
{
  using App_Start;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Web;

  public class AddItemBindingModel
  {
    [Required(ErrorMessage = "Заглавието е задължително")]
    [MinLength(3, ErrorMessage = "Заглавието трябва да е поне 3 символа дълго")]
    [MaxLength(100, ErrorMessage = "Заглавието не трябва да надвишава 100 символа.")]
    [Display(Name = "Заглавие")]
    public string Title { get; set; }

    [Display(Name = "Цена")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Посочете категория")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [MaxLength(1000, ErrorMessage = "Описанието не трябва да надвишава 1000 символа.")]
    [Display(Name = "Описание")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Посочете налично количество")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Required]
    [Display(Name = "Снимки")]
    [DataType(DataType.Upload)]
    public ICollection<HttpPostedFileBase> Pictures { get; set; }
  }

  public class EditItemBindingModel
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Заглавието е задължително")]
    [MinLength(3, ErrorMessage = "Заглавието трябва да е поне 3 символа дълго")]
    [MaxLength(100, ErrorMessage = "Заглавието не трябва да надвишава 100 символа.")]
    [Display(Name = "Заглавие")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Цената е задължителна")]
    [Display(Name = "Цена")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Посочете категория")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [MaxLength(1000, ErrorMessage = "Описанието не трябва да надвишава 1000 символа.")]
    [Display(Name = "Описание")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Снимки")]
    public List<string> OldPictures { get; set; }

    [DataType(DataType.Upload)]
    public ICollection<HttpPostedFileBase> NewPictures { get; set; }
  }
}