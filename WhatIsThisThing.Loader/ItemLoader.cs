using Couchbase.KeyValue;
using WhatIsThisThing.Core.Services;

namespace WhatIsThisThing.Loader
{
    internal class ItemLoader
    {
        public static async Task Load(ICouchbaseCollection itemCollection, IEmbeddingService embed)
        {
            var itemsAlreadyExist = await itemCollection.ExistsAsync("item010");
            if (itemsAlreadyExist.Exists)
                return;
            
            var imagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

            var items = new Dictionary<string, dynamic>();
            var base64image =
                await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "001_reticulated_splines.webp"));
            items.Add("item001", new
            {
                Name = "Reticulated Splines",
                Desc = "Specialized grooves used in advanced machinery for precise alignment.",
                Price = 19.99,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image = await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "002_flux_capacitor.webp"));
            items.Add("item002", new
            {
                Name = "Flux Capacitor",
                Desc = "A component typically used in high-energy equipment for stabilizing power flow.",
                Price = 179.97,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image = await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "003_thermionic_valve.webp"));
            items.Add("item003", new
            {
                Name = "Thermionic Valve",
                Desc = "An electron tube used in high-frequency applications and radio transmitters.",
                Price = 1.99,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image = await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "004_gyro_stabilizer.webp"));
            items.Add("item004", new
            {
                Name = "Gyro Stabilizer",
                Desc = "Device used to maintain balance and stability in moving systems.",
                Price = 3000.18,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image =
                await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "005_quantum_tunneling_composite.webp"));
            items.Add("item005", new
            {
                Name = "Quantum Tunneling Composite",
                Desc = "Material that changes electrical resistance under strain, used in advanced sensors.",
                Price = 328.14,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image =
                await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "006_harmonic_resonator.webp"));
            items.Add("item006", new
            {
                Name = "Harmonic Resonator",
                Desc = "Component that controls vibration frequencies in precision machinery.",
                Price = 25.00,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image = await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "007_cryogenic_coupler.webp"));
            items.Add("item007", new
            {
                Name = "Cryogenic Coupler",
                Desc = "Connects cryogenic lines in ultra-low temperature applications.",
                Price = 148.07,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image =
                await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "008_photoresist_spinner.webp"));
            items.Add("item008", new
            {
                Name = "Photoresist Spinner",
                Desc = "Equipment part used in semiconductor manufacturing to coat wafers with photoresist.",
                Price = 299.99,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image = await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "009_plasma_injector.webp"));
            items.Add("item009", new
            {
                Name = "Plasma Injector",
                Desc = "Device used to introduce plasma into a chamber for materials processing.",
                Price = 399.99,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            base64image =
                await ImageHelper.ImageToBase64Png(Path.Combine(imagesFolderPath, "010_magnetohydrodynamic_pump.webp"));
            items.Add("item010", new
            {
                Name = "Magnetohydrodynamic Pump",
                Desc = "Pump that uses magnetic fields to move conductive fluids without mechanical parts.",
                Price = 5400.00,
                Image = base64image,
                ImageVector = await embed.GetImageEmbedding(base64image)
            });

            foreach (var item in items)
            {
                await itemCollection.UpsertAsync(item.Key, item.Value);
            }
        }
    }
}
