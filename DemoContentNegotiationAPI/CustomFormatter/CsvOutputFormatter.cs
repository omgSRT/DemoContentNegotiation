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
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/xml"));
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
        //        if (context.HttpContext.Request.Headers["Accept"].ToString().Contains("text/csv", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var stringBuilder = new StringBuilder();
        //            if (context.Object is IEnumerable<Blog> blogs)
        //            {
        //                // Write multiple blogs to the CSV
        //                foreach (var blog in blogs)
        //                {
        //                    FormatCsv(stringBuilder, blog);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            await csvWriter.WriteRecordsAsync((IEnumerable)context.Object!);
        //        }
        //    }
        //}
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            await using (var streamWriter = new StreamWriter(context.HttpContext.Response.Body, selectedEncoding, leaveOpen: true))
            {
                var acceptHeader = context.HttpContext.Request.Headers["Accept"].ToString();

                if (acceptHeader.Contains("text/csv", StringComparison.OrdinalIgnoreCase))
                {
                    if (context.Object is IEnumerable<object> objects)
                    {
                        var stringBuilder = new StringBuilder();

                        stringBuilder.AppendLine("Name,Description,BlogPostsDetails"); // Adjust headers as needed

                        foreach (var data in objects)
                        {
                            FormatCsvForBlogObject(stringBuilder, blog: (Blog)data);
                        }

                        await streamWriter.WriteAsync(stringBuilder.ToString());
                    }
                }
                else if (acceptHeader.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
                {
                    // tạo XML response
                    var xmlSerializer = new XmlSerializer(context.Object!.GetType());
                    xmlSerializer.Serialize(streamWriter, context.Object);
                }
                // Header ko hỗ trợ sẽ trả về lỗi 406
            }
        }




        private static void FormatCsvForBlogObject(StringBuilder stringBuilder, Blog blog)
        {
            var blogPostsDetails = string.Join(";", blog.BlogPosts.Select(bp => $"{bp.Title}:{bp.MetaDescription} (Published: {bp.Published})"));

            stringBuilder.AppendLine($"{blog.Name},{blog.Description},\"{blogPostsDetails}\"");
        }
    }
}
