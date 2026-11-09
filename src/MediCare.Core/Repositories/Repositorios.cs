using MediCare.Core.Csv;
using MediCare.Core.Models;

namespace MediCare.Core.Repositories;

public class PacienteRepository(string caminho) : UsuarioRepository<Paciente>(caminho)
{
    protected override string[] ColunasProprias => ["Telefone", "DataNascimento", "CanalPreferido"];

    protected override void LerCamposProprios(Paciente p, IReadOnlyList<string> c)
    {
        p.Telefone = c[0];
        p.DataNascimento = CsvValor.ParaData(c[1]);
        p.CanalPreferido = CsvValor.ParaEnum<CanalNotificacao>(c[2]);
    }

    protected override IEnumerable<string> CamposProprios(Paciente p) =>
        [p.Telefone, CsvValor.De(p.DataNascimento), CsvValor.De(p.CanalPreferido)];
}

public class MedicoRepository(string caminho) : UsuarioRepository<Medico>(caminho)
{
    protected override string[] ColunasProprias => ["Crm", "Telefone", "EspecialidadeId"];

    protected override void LerCamposProprios(Medico m, IReadOnlyList<string> c)
    {
        m.Crm = c[0];
        m.Telefone = c[1];
        m.EspecialidadeId = CsvValor.ParaInt(c[2]);
    }

    protected override IEnumerable<string> CamposProprios(Medico m) =>
        [m.Crm, m.Telefone, CsvValor.De(m.EspecialidadeId)];
}

public class RecepcionistaRepository(string caminho) : UsuarioRepository<Recepcionista>(caminho);

public class AdministradorRepository(string caminho) : UsuarioRepository<Administrador>(caminho);

public class EspecialidadeRepository(string caminho) : CsvRepository<Especialidade>(caminho)
{
    protected override string[] Cabecalho => ["Id", "Nome"];

    protected override Especialidade DeCampos(IReadOnlyList<string> c) => new()
    {
        Id = CsvValor.ParaInt(c[0]),
        Nome = c[1]
    };

    protected override IEnumerable<string> ParaCampos(Especialidade e) => [CsvValor.De(e.Id), e.Nome];
}

public class LocalAtendimentoRepository(string caminho) : CsvRepository<LocalAtendimento>(caminho)
{
    protected override string[] Cabecalho => ["Id", "MedicoId", "Nome", "Endereco", "Cidade"];

    protected override LocalAtendimento DeCampos(IReadOnlyList<string> c) => new()
    {
        Id = CsvValor.ParaInt(c[0]),
        MedicoId = CsvValor.ParaInt(c[1]),
        Nome = c[2],
        Endereco = c[3],
        Cidade = c[4]
    };

    protected override IEnumerable<string> ParaCampos(LocalAtendimento l) =>
        [CsvValor.De(l.Id), CsvValor.De(l.MedicoId), l.Nome, l.Endereco, l.Cidade];
}

public class HorarioRepository(string caminho) : CsvRepository<HorarioDisponivel>(caminho)
{
    protected override string[] Cabecalho =>
        ["Id", "MedicoId", "Data", "HoraInicio", "HoraFim", "Modalidade", "LocalId", "Disponivel"];

    protected override HorarioDisponivel DeCampos(IReadOnlyList<string> c)
    {
        var horario = new HorarioDisponivel
        {
            Id = CsvValor.ParaInt(c[0]),
            MedicoId = CsvValor.ParaInt(c[1]),
            Data = CsvValor.ParaData(c[2]),
            HoraInicio = CsvValor.ParaHora(c[3]),
            HoraFim = CsvValor.ParaHora(c[4]),
            Modalidade = CsvValor.ParaEnum<Modalidade>(c[5]),
            LocalId = CsvValor.ParaIntOpcional(c[6])
        };
        horario.Restaurar(CsvValor.ParaBool(c[7]));
        return horario;
    }

    protected override IEnumerable<string> ParaCampos(HorarioDisponivel h) =>
    [
        CsvValor.De(h.Id), CsvValor.De(h.MedicoId), CsvValor.De(h.Data), CsvValor.De(h.HoraInicio),
        CsvValor.De(h.HoraFim), CsvValor.De(h.Modalidade), CsvValor.De(h.LocalId), CsvValor.De(h.Disponivel)
    ];
}

public class ConsultaRepository(string caminho) : CsvRepository<Consulta>(caminho)
{
    protected override string[] Cabecalho => ["Id", "PacienteId", "HorarioId", "Status", "DataAgendamento"];

    protected override Consulta DeCampos(IReadOnlyList<string> c) => new(
        id: CsvValor.ParaInt(c[0]),
        pacienteId: CsvValor.ParaInt(c[1]),
        horarioId: CsvValor.ParaInt(c[2]),
        status: CsvValor.ParaEnum<StatusConsulta>(c[3]),
        dataAgendamento: CsvValor.ParaDataHora(c[4]));

    protected override IEnumerable<string> ParaCampos(Consulta c) =>
    [
        CsvValor.De(c.Id), CsvValor.De(c.PacienteId), CsvValor.De(c.HorarioId),
        CsvValor.De(c.Status), CsvValor.De(c.DataAgendamento)
    ];
}

public class NotificacaoRepository(string caminho) : CsvRepository<Notificacao>(caminho)
{
    protected override string[] Cabecalho =>
        ["Id", "ConsultaId", "Tipo", "Canal", "Destino", "Mensagem", "DataCriacao", "Status", "DataEnvio"];

    protected override Notificacao DeCampos(IReadOnlyList<string> c) => new(
        id: CsvValor.ParaInt(c[0]),
        consultaId: CsvValor.ParaInt(c[1]),
        tipo: CsvValor.ParaEnum<TipoNotificacao>(c[2]),
        canal: CsvValor.ParaEnum<CanalNotificacao>(c[3]),
        destino: c[4],
        mensagem: c[5],
        dataCriacao: CsvValor.ParaDataHora(c[6]),
        status: CsvValor.ParaEnum<StatusNotificacao>(c[7]),
        dataEnvio: CsvValor.ParaDataHoraOpcional(c[8]));

    protected override IEnumerable<string> ParaCampos(Notificacao n) =>
    [
        CsvValor.De(n.Id), CsvValor.De(n.ConsultaId), CsvValor.De(n.Tipo), CsvValor.De(n.Canal), n.Destino,
        n.Mensagem, CsvValor.De(n.DataCriacao), CsvValor.De(n.Status), CsvValor.De(n.DataEnvio)
    ];
}
