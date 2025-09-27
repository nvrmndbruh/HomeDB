using HomeDB.Data;
using HomeDB.Data.Repositories;
using HomeDB.ViewModels;
using Microsoft.Extensions.Logging;
using UraniumUI;

namespace HomeDB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });

            //builder.Services.AddSingleton<ItemRepository>();
            //builder.Services.AddSingleton<ContainerRepository>();
            //builder.Services.AddSingleton<CategoryRepository>();
            //builder.Services.AddSingleton<HierarchyRepository>();
            //builder.Services.AddSingleton<ItemContainerRepository>();
            //builder.Services.AddSingleton<ItemCategoryRepository>();
            builder.Services.AddSingleton<DatabaseContext>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<EditCategoryPage>();
            builder.Services.AddTransient<EditCategoryViewModel>();

            builder.Services.AddTransient<CategoryPage>();
            builder.Services.AddTransient<CategoryViewModel>();

            builder.Services.AddTransient<ItemListPage>();
            builder.Services.AddTransient<ItemListViewModel>();

            builder.Services.AddTransient<EditItemPage>();
            builder.Services.AddTransient<EditItemViewModel>();

            builder.Services.AddTransient<EditContainerPage>();
            builder.Services.AddTransient<EditContainerViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}