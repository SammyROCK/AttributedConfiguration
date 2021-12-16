# AttributedConfiguration
Fornece configuração por atributos para projetos NetCore.

### O que você vai encontrar aqui:
  - Instalação
  - Como usar
  - Exemplo

## Instalação

Este projeto requer:
  - Instalar o NuGet conforme indicado na [WiKi](https://wiki.cappta.com.br/pt-br/Tecnologia/GitHub/Packages/Nuget)

## Como usar
### Registrar AttributedConfiguration:
No arquivo `Startup.cs` adicione a diretiva `using` para o pacote e no método `ConfigureServices` adicione o serviço.

```c#
using AttributedConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
			=> this.configuration = configuration;

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAttributedConfigurations(this.configuration);
		}
	}
}
```

### Desenvolver classes de configuração:
Adicione a diretiva `using` para o pacote, o atributo `Configure` e faça a classe herdar da `BaseConfiguration` de `AttributedConfiguration`:

```c#
using AttributedConfiguration;
using Microsoft.Extensions.Configuration;

namespace Parselmouth.Configuration {
	[Configure("[Configuration]")]
	public class ElasticSearchConfiguration : BaseConfiguration {
		public ElasticSearchConfiguration(IConfiguration configuration)
			: base(configuration) {
			this.Server = this.GetString(nameof(this.Server));
			this.Index = this.GetString(nameof(this.Index));
		}

		public string Server { get; }

		public string Index { get; }
	}
}
```
## Exemplo
<details> <summary>Veja aqui um exemplo de uso</summary>

```c#
using AttributedConfiguration;
using Microsoft.Extensions.Configuration;
using System;

namespace Parselmouth.Configuration {
	public interface IPinConfiguration {
		int Count { get; }
		TimeSpan Duration { get; }
	}

	[Configure("[Configuration]")]
	public class PinConfiguration : BaseConfiguration, IPinConfiguration {
		public PinConfiguration(IConfiguration configuration) : base(configuration) {
			this.Count = this.GetInt(nameof(this.Count));
			this.Duration = this.GetTimespan(nameof(this.Duration), TimeSource.InMinutes);
		}

		public int Count { get; set; }
		public TimeSpan Duration { get; set; }
	}
}
```

</details>
