using CsvHelper;
using CsvHelper.Configuration;
using DemoContentNegotiationAPI.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace DemoContentNegotiationAPI.CustomFormatter
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter() {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        protected override bool CanWriteType(Type? type)
        {
            if (typeof(IEnumerable<Blog>).IsAssignableFrom(type) || typeof(Blog).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }
        //public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        //{
        //    await using (var streamWriter = new StreamWriter(context.HttpContext.Response.Body, selectedEncoding, leaveOpen: true))
        //    await using (var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
        //    {
        //        await csvWriter.WriteRecordsAsync((IEnumerable)context.Object!);
        //    }
        //}
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            await using (var streamWriter = new StreamWriter(context.HttpContext.Response.Body, selectedEncoding, leaveOpen: true))
            {
                var stringBuilder = new StringBuilder();

                if (context.Object is IEnumerable<Blog> blogs)
                {
                    stringBuilder.AppendLine("Name,Description,BlogPostsDetails"); // Adjust headers as needed

                    foreach (var data in blogs)
                    {
                        FormatCsv(stringBuilder, blog: (Blog)data);
                    }
                }
                else if(context.Object is Blog blog)
                {
                    stringBuilder.AppendLine("Name,Description,BlogPostsDetails");
                    FormatCsv(stringBuilder, blog);
                }
                await streamWriter.WriteAsync(stringBuilder.ToString());
            }
        }

        private static void FormatCsv(StringBuilder stringBuilder, Blog blog)
        {
            var blogPostsDetails = string.Join(";", blog.BlogPosts.Select(bp => $"{bp.Title}:{bp.MetaDescription} (Published: {bp.Published})"));

            stringBuilder.AppendLine($"{blog.Name},{blog.Description},\"{blogPostsDetails}\"");
        }
    }
}
