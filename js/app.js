const { useEffect, useState } = React;

const API_BASE_URL = window.LEAD_MANAGER_API ?? 'http://localhost:5088/api';

const TABS = [
  {
    key: 'Invited',
    label: 'Invited',
    description: 'Leads aguardando sua resposta.',
    icon: 'fa-envelope-open-text'
  },
  {
    key: 'Accepted',
    label: 'Accepted',
    description: 'Leads já aceitos pela equipe.',
    icon: 'fa-circle-check'
  }
];

const formatDate = (value) =>
  new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(value));

const formatPrice = (value) =>
  new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2
  }).format(value);

const MetaItem = ({ label, value, icon }) => (
  <div className="meta-item">
    <small><i className={`fa-solid ${icon}`} style={{ marginRight: '6px' }} />{label}</small>
    <span>{value}</span>
  </div>
);

const LeadCard = ({ lead, onAccept, onDecline, isProcessing, showActions, isAcceptedTab }) => {
  const statusChipLabel = lead.status;

  return (
    <article className="lead-card">
      <div className="lead-header">
        <h2>{lead.contactFullName || lead.contactFirstName}</h2>
        <span className="status-chip">
          <i className="fa-solid fa-bolt" style={{ marginRight: '6px' }} />
          {statusChipLabel}
        </span>
      </div>

      <div className="lead-meta">
        <MetaItem label="Criado em" value={formatDate(lead.createdAt)} icon="fa-calendar" />
        <MetaItem label="Bairro" value={lead.suburb} icon="fa-location-dot" />
        <MetaItem label="Categoria" value={lead.category} icon="fa-layer-group" />
        <MetaItem label="ID" value={`#${lead.id}`} icon="fa-hashtag" />
      </div>

      <p className="description">{lead.description}</p>

      {isAcceptedTab && (
        <div className="lead-meta">
          <MetaItem label="Telefone" value={lead.contactPhoneNumber || 'Não informado'} icon="fa-phone" />
          <MetaItem label="Email" value={lead.contactEmail || 'Não informado'} icon="fa-envelope" />
        </div>
      )}

      <div className="card-footer">
        <span className="price">{formatPrice(lead.price)}</span>
        {showActions && (
          <div className="actions">
            <button
              className="accept"
              disabled={isProcessing}
              onClick={() => onAccept(lead.id)}
            >
              <i className="fa-solid fa-circle-check" /> Aceitar
            </button>
            <button
              className="decline"
              disabled={isProcessing}
              onClick={() => onDecline(lead.id)}
            >
              <i className="fa-solid fa-circle-xmark" /> Recusar
            </button>
          </div>
        )}
      </div>
    </article>
  );
};

const LeadManagementApp = () => {
  const [activeTab, setActiveTab] = useState('Invited');
  const [leads, setLeads] = useState({ Invited: [], Accepted: [] });
  const [loadedTabs, setLoadedTabs] = useState({ Invited: false, Accepted: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [actionState, setActionState] = useState(null);

  const fetchLeads = async (status) => {
    setLoading(true);
    setError(null);
    try {
      const response = await axios.get(`${API_BASE_URL}/leads`, { params: { status } });
      setLeads((prev) => ({ ...prev, [status]: response.data }));
      setLoadedTabs((prev) => ({ ...prev, [status]: true }));
    } catch (err) {
      console.error(err);
      setError('Não foi possível carregar os leads. Verifique se a API está em execução.');
    } finally {
      setLoading(false);
    }
  };

  const isTabLoaded = loadedTabs[activeTab];

  useEffect(() => {
    if (!isTabLoaded) {
      fetchLeads(activeTab);
    }
  }, [activeTab, isTabLoaded]);

  const handleAction = async (leadId, action) => {
    setActionState({ leadId, action });
    setError(null);

    try {
      const response = await axios.post(`${API_BASE_URL}/leads/${leadId}/${action}`);
      const updatedLead = response.data;

      setLeads((prev) => {
        const next = {
          Invited: [...prev.Invited],
          Accepted: [...prev.Accepted]
        };

        if (action === 'accept') {
          next.Invited = next.Invited.filter((lead) => lead.id !== leadId);
          const withoutLead = next.Accepted.filter((lead) => lead.id !== leadId);
          next.Accepted = [updatedLead, ...withoutLead];
        } else {
          next.Invited = next.Invited.filter((lead) => lead.id !== leadId);
        }

        return next;
      });
    } catch (err) {
      console.error(err);
      setError('Não foi possível atualizar o lead. Tente novamente.');
    } finally {
      setActionState(null);
    }
  };

  const handleAccept = (leadId) => handleAction(leadId, 'accept');
  const handleDecline = (leadId) => handleAction(leadId, 'decline');

  const displayedLeads = leads[activeTab] ?? [];

  return (
    <>
      <header>
        <div>
          <h1>Lead Manager</h1>
          <p style={{ margin: 0, color: 'var(--muted)' }}>
            Controle centralizado dos leads da sua equipe de vendas.
          </p>
        </div>
        <i className="fa-solid fa-users-viewfinder" style={{ fontSize: '2rem', color: 'var(--primary)' }} />
      </header>
      <main className="container">
        {error && <div className="error-message">{error}</div>}

        <nav className="tabs" aria-label="Navegação de status">
          {TABS.map((tab) => (
            <button
              key={tab.key}
              className={`tab-button ${tab.key === activeTab ? 'active' : ''}`}
              onClick={() => setActiveTab(tab.key)}
            >
              <i className={`fa-solid ${tab.icon}`} style={{ marginRight: '8px' }} />
              {tab.label}
            </button>
          ))}
        </nav>

        <p style={{ marginTop: '-0.5rem', marginBottom: '1.5rem', color: 'var(--muted)' }}>
          {TABS.find((tab) => tab.key === activeTab)?.description}
        </p>

        {loading ? (
          <div className="loading">
            <i className="fa-solid fa-spinner fa-spin" style={{ marginRight: '8px' }} />
            Carregando leads...
          </div>
        ) : displayedLeads.length === 0 ? (
          <div className="empty-state">
            <i className="fa-solid fa-inbox" style={{ fontSize: '2rem', marginBottom: '1rem' }} />
            <p>Nenhum lead encontrado para este status.</p>
          </div>
        ) : (
          <section className="leads-grid">
            {displayedLeads.map((lead) => (
              <LeadCard
                key={lead.id}
                lead={lead}
                onAccept={handleAccept}
                onDecline={handleDecline}
                isProcessing={actionState?.leadId === lead.id}
                showActions={activeTab === 'Invited'}
                isAcceptedTab={activeTab === 'Accepted'}
              />
            ))}
          </section>
        )}
      </main>
    </>
  );
};

ReactDOM.createRoot(document.getElementById('root')).render(<LeadManagementApp />);
