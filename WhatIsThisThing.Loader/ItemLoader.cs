using Couchbase.KeyValue;
using WhatIsThisThing.Core.Services;

namespace WhatIsThisThing.Loader
{
    internal class ItemLoader
    {
        private readonly ICouchbaseCollection _itemCollection;
        private readonly IEmbeddingService _embed;
        private readonly ICouchbaseCollection _stockCollection;

        public ItemLoader(ICouchbaseCollection itemCollection, IEmbeddingService embed, ICouchbaseCollection stockCollection)
        {
            _itemCollection = itemCollection;
            _embed = embed;
            _stockCollection = stockCollection;
        }
        
        public async Task Load()
        {
            var imagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

            await LoadItem(
                key: "item001",
                name: "Reticulated Splines",
                desc: "Specialized grooves used in advanced machinery for precise alignment.",
                price: 19.99,
                rating: 5,
                imagePath: Path.Combine(imagesFolderPath, "001_reticulated_splines.webp")
            );
            await LoadItem(
                key: "item002",
                name: "Flux Capacitor",
                desc: "A component typically used in high-energy equipment for stabilizing power flow.",
                price: 1985.55,
                rating: 5,
                imagePath: Path.Combine(imagesFolderPath, "002_flux_capacitor.webp")
            );

            await LoadItem(
                key: "item003",
                name: "Thermionic Valve",
                desc: "An electron tube used in high-frequency applications and radio transmitters.",
                price: 1.99,
                rating: 4,
                imagePath: Path.Combine(imagesFolderPath, "003_thermionic_valve.webp")
            );

            await LoadItem(
                key: "item004",
                name: "Gyro Stabilizer",
                desc: "Device used to maintain balance and stability in moving systems.",
                price: 3000.18,
                rating: 4,
                imagePath: Path.Combine(imagesFolderPath, "004_gyro_stabilizer.webp")
            );

            await LoadItem(
                key: "item005",
                name: "Quantum Tunneling Composite",
                desc: "Material that changes electrical resistance under strain, used in advanced sensors.",
                price: 328.14,
                rating: 4,
                imagePath: Path.Combine(imagesFolderPath, "005_quantum_tunneling_composite.webp")
            );

            await LoadItem(
                key: "item006",
                name: "Harmonic Resonator",
                desc: "Component that controls vibration frequencies in precision machinery.",
                price: 25.00,
                rating: 4,
                imagePath: Path.Combine(imagesFolderPath, "006_harmonic_resonator.webp")
            );

            await LoadItem(
                key: "item007",
                name: "Cryogenic Coupler",
                desc: "Connects cryogenic lines in ultra-low temperature applications.",
                price: 148.07,
                rating: 3,
                imagePath: Path.Combine(imagesFolderPath, "007_cryogenic_coupler.webp")
            );

            await LoadItem(
                key: "item008",
                name: "Photoresist Spinner",
                desc: "Equipment part used in semiconductor manufacturing to coat wafers with photoresist.",
                price: 299.99,
                rating: 3,
                imagePath: Path.Combine(imagesFolderPath, "008_photoresist_spinner.webp")
            );

            await LoadItem(
                key: "item009",
                name: "Plasma Injector",
                desc: "Device used to introduce plasma into a chamber for materials processing.",
                price: 399.99,
                rating: 3,
                imagePath: Path.Combine(imagesFolderPath, "009_plasma_injector.webp")
            );

            await LoadItem(
                key: "item010",
                name: "Magnetohydrodynamic Pump",
                desc: "Pump that uses magnetic fields to move conductive fluids without mechanical parts.",
                price: 5400.00,
                rating: 3,
                imagePath: Path.Combine(imagesFolderPath, "010_magnetohydrodynamic_pump.webp")
            );

            await LoadItem(
                key: "item011",
                name: "Optical Isolator",
                desc: "Device used to prevent back reflection in optical systems, ensuring signal integrity.",
                price: 75.50,
                rating: 2,
                imagePath: Path.Combine(imagesFolderPath, "011_optical_isolator.webp")
            );

            await LoadItem(
                key: "item012",
                name: "Nano-Carbon Actuator",
                desc: "High-precision actuator used in nanotechnology applications for fine movement control.",
                price: 450.00,
                rating: 2,
                imagePath: Path.Combine(imagesFolderPath, "012_nano_carbon_actuator.webp")
            );

            await LoadItem(
                key: "item013",
                name: "Superconducting Coil",
                desc: "Coil used in superconducting magnets for generating strong magnetic fields with minimal energy loss.",
                price: 12000.00,
                rating: 2,
                imagePath: Path.Combine(imagesFolderPath, "013_superconducting_coil.webp")
            );

            await LoadItem(
                key: "item014",
                name: "Piezoelectric Transducer",
                desc: "Device that converts electrical energy into mechanical motion and vice versa, used in sensors and actuators.",
                price: 89.99,
                rating: 1,
                imagePath: Path.Combine(imagesFolderPath, "014_piezoelectric_transducer.webp")
            );

            await LoadItem(
                key: "item015",
                name: "Graphene Membrane",
                desc: "Ultra-thin, strong, and conductive membrane used in advanced filtration and electronic applications.",
                price: 299.99,
                rating: 1,
                imagePath: Path.Combine(imagesFolderPath, "015_graphene_membrane.webp")
            );

            await LoadCouchbaseItem(
                key: "item016",
                name: "Couchbase Pen",
                desc: "A sleek black pen with silver accents, featuring the Couchbase logo, perfect for everyday writing with a professional touch.",
                price: 1.79,
                imagePath: Path.Combine(imagesFolderPath, "016_couchbase_pen.jpg")
            );

            await LoadCouchbaseItem(
                key: "item017",
                name: "Couchbase Carabiner",
                desc: "A sturdy metallic carabiner with a compact design, featuring the Couchbase logo, ideal for securing keys or gear on the go.",
                price: 1.99,
                imagePath: Path.Combine(imagesFolderPath, "017_couchbase_carabiner.jpg")
            );

            await LoadCouchbaseItem(
                key: "item018",
                name: "Couchbase SD Card",
                desc: "A 4GB PNY Premium SD card preloaded with Couchbase software, ready for quick installation on compatible devices.",
                price: 0.99,
                imagePath: Path.Combine(imagesFolderPath, "018_sd_card.jpg")
            );

            await LoadCouchbaseItem(
                key: "item019",
                name: "Couchbase Phone Mount",
                desc: "A red adjustable plastic ball with a magnetic phone mount, featuring the Couchbase logo, ideal for hands-free use and secure phone positioning.",
                price: 4.99,
                imagePath: Path.Combine(imagesFolderPath, "019_couchbase_phone_mount.jpg")
            );

        }

        private async Task LoadCouchbaseItem(string key, string name, string desc, double price, string imagePath)
        {
            var itemExist = await _itemCollection.ExistsAsync(key);
            if (itemExist.Exists)
                return;

            var base64image = await ImageHelper.ImageToBase64Png(imagePath);

            var item = new
            {
                Name = name,
                Desc = desc,
                Price = price,
                Image = base64image,
                Rating = 5,
                ImageVector = await _embed.GetImageEmbedding(base64image)
            };

            await _itemCollection.UpsertAsync(key, item);

            var numInStock = new Random().Next(1, 200);

            await StockLoader.LoadStock(_stockCollection, "store999", key, numInStock);
        }


        private async Task LoadItem(string key, string name, string desc, double price, string imagePath, int rating)
        {
            var itemExist = await _itemCollection.ExistsAsync(key);
            if (itemExist.Exists)
                return;
            
            var base64image = await ImageHelper.ImageToBase64Png(imagePath);

            var item = new
            {
                Name = name,
                Desc = desc,
                Price = price,
                Image = base64image,
                Rating = rating,
                ImageVector = await _embed.GetImageEmbedding(base64image)
            };

            await _itemCollection.UpsertAsync(key, item);
        }
    }
}
