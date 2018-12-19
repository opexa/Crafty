namespace Crafty.App.App_Start
{
  using System;
  using System.Web.Mvc;

  public static class ExtensionMethods
  {
    public static MvcHtmlString Details(this HtmlHelper html, string detailsText)
    {
      string inText = detailsText.Replace(Environment.NewLine, " </br> ");
      inText = inText.Replace("\n", " </br> ");
      string[] words = inText.Split(new[] { ' ' });
      
      for(int i = 0; i < words.Length; i++)
      {
        if(words[i].Contains("http://") || words[i].Contains("www."))
        {
          if(words[i].Contains("www.") && !words[i].Contains("http://"))
          {
            words[i] = "<a target='_blank' class='intext-link' href='http://" + words[i] + "'>" + words[i] + "</a>";
          }
          else
          {
            words[i] = "<a target='_blank' class='intext-link' href='" + words[i] + "'>" + words[i] + "</a>";
          }
        }
      }
      string text = String.Join(" ", words);
      return MvcHtmlString.Create(text);
    }
  }
}