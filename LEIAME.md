# NeuroRotina

Aplicativo de apoio à comunicação e rotina para crianças autistas não verbais.
Projeto desenvolvido em **.NET MAUI** a partir do PTCC da equipe (Fabiana, Hanna,
Andréa, Paolla e Lucicléia).

---

## Como abrir no Visual Studio

1. Instale o **Visual Studio 2026** com a carga de trabalho
   *Desenvolvimento para dispositivos móveis com .NET* (.NET MAUI). O .NET 10 SDK
   é instalado junto pelo instalador.
2. Abra o arquivo `NeuroRotina.sln`.
3. Aguarde a restauração dos pacotes NuGet (`sqlite-net-pcl` e `SQLitePCLRaw.bundle_green`).
   Se não restaurar sozinho: botão direito na solução → **Restaurar pacotes NuGet**.
4. Escolha o destino na barra superior:
   - **Windows Machine** — mais rápido para testar durante o desenvolvimento;
   - **Android Emulator** ou um celular com depuração USB ativada — necessário para
     testar voz, vibração e escolha de fotos como no uso real.
5. Pressione **F5**.

Pela linha de comando:

```bash
dotnet restore
dotnet build -f net10.0-android
dotnet build -f net10.0-windows10.0.19041.0
```

> O projeto está em **.NET 10**, a versão que acompanha o Visual Studio 2026.
> Para adicionar iOS/macOS, inclua `net10.0-ios;net10.0-maccatalyst` na mesma linha
> de `TargetFrameworks` e crie a pasta `Platforms/iOS` com os arquivos padrão do
> modelo MAUI.

---

## Primeiro uso

1. Toque em **Criar uma conta** e cadastre o responsável.
2. Cadastre a criança. Ela já nasce com 12 cartões de comunicação prontos.
3. Na aba **Rotinas**, crie a primeira rotina (ex.: "Manhã", 07:00) e ative o
   aviso de transição.
4. Abra a rotina → **Passos da rotina** → adicione os passos com pictograma e
   a frase que será falada.
5. Na aba **Hoje**, toque em **Abrir modo criança** e entregue o aparelho à criança.

---

## Estrutura do código

```
NeuroRotina/
├── Models/          Entidades do diagrama de classes (Usuario, Crianca, Rotina,
│                    Atividade, Alerta, Historico) + Cartao da prancha
├── Services/        Banco SQLite, sessão, mídia (voz/foto/vibração) e alertas
├── Views/           Telas em XAML com code-behind
├── Converters/      Conversores usados nos bindings
├── Resources/       Paleta, estilos, ícone e splash
└── Platforms/       Código específico de Android e Windows
```

### Correspondência com o diagrama de classes

| Diagrama | Implementação |
|---|---|
| `Usuario.login()` | `DatabaseService.AutenticarAsync` |
| `Usuario.cadastrar()` | `CadastroPage` + `DatabaseService.SalvarUsuarioAsync` |
| `Usuario.editarPerfil()` | `PerfilPage` |
| `Cuidador.criarRotina()` | `RotinaEditarPage` |
| `Cuidador.cadastrarAtividade()` | `AtividadeEditarPage` |
| `Cuidador.gerenciarRotina()` | `RotinasPage` + `AtividadesPage` |
| `Cuidador.visualizarHistorico()` | `HistoricoPage` |
| `Cuidador.receberAlertas()` | `AlertaService` + painel de aviso na `HomePage` |
| `Crianca.visualizarRotina()` | `ModoCriancaPage` |
| `Crianca.reproduzirAudio()` | `MidiaService.FalarAsync` (síntese de voz do sistema) |
| `Crianca.comunicarImagem()` | `ComunicacaoPage` |
| `Rotina.adicionarAtividade()` / `removerAtividade()` / `listarRotina()` | `AtividadesPage` |
| `Atividade.exibirImagem()` / `tocarAudio()` | `ModoCriancaPage` |
| `Alerta.enviarNotificacao()` | `AlertaService.VerificarAsync` |
| `Historico.registrarComportamento()` | `HistoricoPage` |

### Correspondência com o backlog

| Sprint | Entrega | Onde está |
|---|---|---|
| 2 | Configuração do projeto e navegação | `NeuroRotina.csproj`, `MauiProgram.cs`, `AppShell.xaml` |
| 3 | Login, cadastro e edição de perfil | `LoginPage`, `CadastroPage`, `PerfilPage` |
| 4 | Rotinas visuais, atividades, imagens e ícones | `RotinaEditarPage`, `AtividadesPage`, `AtividadeEditarPage` |
| 5 | Comunicação por imagens, alertas e notificações visuais | `ComunicacaoPage`, `AlertaService`, painel de aviso da `HomePage` |
| 6 | Acessibilidade e ajustes de usabilidade | Estilos em `Resources/Styles`, `ModoCriancaPage` |

---

## Decisões técnicas

**Banco local SQLite.** Atende à restrição de não haver orçamento para
infraestrutura paga e mantém dados sensíveis de uma criança no próprio aparelho.
O arquivo fica em `FileSystem.AppDataDirectory/neurorotina.db3`.

**Senha em hash SHA-256.** A senha digitada nunca é gravada em texto puro
(`Services/SenhaHelper.cs`).

**Voz pela síntese nativa (`TextToSpeech`).** Evita depender de arquivos de áudio
gravados e funciona em qualquer texto que o cuidador escrever. O campo de áudio
de cada atividade continua no modelo, permitindo trocar por arquivos `.mp3` no futuro.

**Pictogramas em vez de fotos obrigatórias.** O aplicativo já é usável sem que o
cuidador precise fotografar nada; a foto é opcional e, quando escolhida, é copiada
para a pasta do aplicativo para não perder o acesso depois.

**Paleta de baixa estimulação.** Fundo neutro, azul-petróleo na navegação e âmbar
apenas no que exige atenção. Alvos de toque nunca menores que 52 pixels
independentes, e o modo criança usa um passo por tela para reduzir a carga visual.

---

## Limitações conhecidas (próximos passos)

- Os alertas só disparam com o aplicativo aberto. Para notificações com o app
  fechado, adicione o pacote `Plugin.LocalNotification` e agende as notificações
  em `AlertaService`.
- Não há sincronização entre aparelhos nem backup na nuvem.
- Não há exportação do histórico em PDF ou planilha.
- iOS e macOS não estão configurados neste projeto.
