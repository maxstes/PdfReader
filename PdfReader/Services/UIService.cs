﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfReader.Services
{
    internal class UIService
    {
        private readonly MainPageReader mainPageReader = new();

        public UIService()
        {
        }
        private int СhouseSection(int countsElems)
        {
            int id = Convert.ToInt32(Console.ReadLine());   
            return id;
        }
        public async Task<string> GetUrlChooseSection()
        {
            var sections = await mainPageReader.MainFunc();
            int id = 0;
            foreach (var element in sections)
            {
                await Console.Out.WriteLineAsync($"Id: {id} Title: {element.title}");
                id++;
            }
            id = СhouseSection(sections.Count);
            
            string url = sections[id].url;
            return url;
        }

    }
}
