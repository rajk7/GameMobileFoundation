﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pancake.BakingSheet.Internal;
using Pancake.BakingSheet.Raw;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GSheet = Google.Apis.Sheets.v4.Data.Sheet;

namespace Pancake.BakingSheet
{
    public class GoogleSheetConverter : RawSheetImporter
    {
        private readonly string _gsheetAddress;
        private readonly ICredential _credential;
        private Spreadsheet _spreadsheet;
        private readonly Dictionary<string, List<Page>> _pages;

        public GoogleSheetConverter(string gsheetAddress, string credential, TimeZoneInfo timeZoneInfo = null, IFormatProvider formatProvider = null)
            : base(timeZoneInfo, formatProvider)
        {
            _gsheetAddress = gsheetAddress;
            _credential = GoogleCredential.FromJson(credential).CreateScoped(DriveService.Scope.DriveReadonly);
            _pages = new Dictionary<string, List<Page>>();
        }

        public async Task<DateTime> FetchModifiedTime()
        {
            using (var service = new DriveService(new BaseClientService.Initializer() {HttpClientInitializer = _credential}))
            {
                var fileReq = service.Files.Get(_gsheetAddress);
                fileReq.SupportsTeamDrives = true;
                fileReq.Fields = "modifiedTime";

                var file = await fileReq.ExecuteAsync();
                return file.ModifiedTime ?? default;
            }
        }

        public override void Reset()
        {
            base.Reset();
            _pages.Clear();
        }

        protected override async Task<bool> LoadData()
        {
            using (var service = new SheetsService(new BaseClientService.Initializer() {HttpClientInitializer = _credential}))
            {
                var sheetReq = service.Spreadsheets.Get(_gsheetAddress);
                sheetReq.Fields = "sheets(properties.title,data.rowData.values.formattedValue)";
                _spreadsheet = await sheetReq.ExecuteAsync();
            }

            _pages.Clear();

            foreach (var gSheet in _spreadsheet.Sheets)
            {
                if (gSheet.Properties.Title.StartsWith(Config.COMMENT)) continue;

                var (sheetName, subName) = Config.ParseSheetName(gSheet.Properties.Title);

                if (!_pages.TryGetValue(sheetName, out var sheetList))
                {
                    sheetList = new List<Page>();
                    _pages.Add(sheetName, sheetList);
                }

                sheetList.Add(new Page(gSheet, subName));
            }

            return true;
        }

        private class Page : IRawSheetImporterPage
        {
            private readonly GridData _grid;

            public string SubName { get; }

            public Page(GSheet gSheet, string subName)
            {
                _grid = gSheet.Data.First();
                SubName = subName;
            }

            public string GetCell(int col, int row)
            {
                if (row >= _grid.RowData.Count || col >= _grid.RowData[row].Values?.Count) return null;

                var value = _grid.RowData[row].Values?[col];
                return value?.FormattedValue;
            }
        }

        protected override IEnumerable<IRawSheetImporterPage> GetPages(string sheetName)
        {
            if (_pages.TryGetValue(sheetName, out var pages)) return pages;
            return Enumerable.Empty<IRawSheetImporterPage>();
        }
    }
}