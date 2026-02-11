import 'package:flutter/material.dart';
import '../../../../../core/services/services_repository.dart';
import '../../../../../core/services/visits_service.dart';
import '../../../../../features/visits/data/models/visit_model.dart';

class VisitsAgendaScreen extends StatelessWidget {
  const VisitsAgendaScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return _VisitsAgendaScreenBody();
  }
}

class _VisitsAgendaScreenBody extends StatefulWidget {
  @override
  State<_VisitsAgendaScreenBody> createState() => _VisitsAgendaScreenBodyState();
}

class _VisitsAgendaScreenBodyState extends State<_VisitsAgendaScreenBody> {
  List<VisitModel> _visits = [];
  bool _loading = true;
  @override
  void initState() {
    super.initState();
    _loadVisits();
  }

  Future<void> _loadVisits() async {
    setState(() => _loading = true);
    final visitsService = ServicesRepository().visitsService;
    try {
      final visits = await visitsService.getVisits();
      setState(() {
        _visits = visits;
        _loading = false;
      });
    } catch (e) {
      setState(() => _loading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error cargando visitas: $e'), backgroundColor: Colors.red),
      );
    }
  }

  void _showVisitForm({VisitModel? visit}) {
    showDialog(
      context: context,
      builder: (context) {
        final contactController = TextEditingController(text: visit?.contactName ?? '');
        final dateController = TextEditingController(text: visit != null ? visit.scheduledDate.toString().split(' ')[0] : '');
        final hourController = TextEditingController(text: visit != null ? visit.scheduledDate.hour.toString().padLeft(2, '0') + ':' + visit.scheduledDate.minute.toString().padLeft(2, '0') : '');
        final statusController = TextEditingController(text: visit?.status ?? 'Programada');
        final durationController = TextEditingController(text: ''); // TODO: Mapear duración si existe
        final noteController = TextEditingController(text: visit?.notes ?? '');
        return AlertDialog(
          title: Text(visit == null ? 'Agendar Visita' : 'Editar Visita'),
          content: SingleChildScrollView(
            child: Column(
              children: [
                TextField(
                  controller: contactController,
                  decoration: const InputDecoration(labelText: 'Contacto'),
                ),
                TextField(
                  controller: dateController,
                  decoration: const InputDecoration(labelText: 'Fecha (YYYY-MM-DD)'),
                ),
                TextField(
                  controller: hourController,
                  decoration: const InputDecoration(labelText: 'Hora (HH:MM)'),
                ),
                TextField(
                  controller: statusController,
                  decoration: const InputDecoration(labelText: 'Estado'),
                ),
                TextField(
                  controller: durationController,
                  decoration: const InputDecoration(labelText: 'Duración'),
                ),
                TextField(
                  controller: noteController,
                  decoration: const InputDecoration(labelText: 'Nota'),
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancelar'),
            ),
            ElevatedButton(
              onPressed: () async {
                // Guardar visita
                final contactName = contactController.text;
                final dateStr = dateController.text;
                final hourStr = hourController.text;
                final status = statusController.text;
                final notes = noteController.text;
                final duration = durationController.text;
                DateTime scheduledDate;
                try {
                  scheduledDate = DateTime.parse(dateStr + ' ' + hourStr);
                } catch (_) {
                  scheduledDate = DateTime.now();
                }
                final visitsService = ServicesRepository().visitsService;
                final userId = ServicesRepository().currentUserId ?? 0;
                final visitModel = VisitModel(
                  visitId: visit?.visitId ?? 0,
                  userId: userId,
                  contactId: 0, // TODO: Mapear contactId real
                  scheduledDate: scheduledDate,
                  status: status,
                  notes: notes,
                  createdAt: visit?.createdAt ?? DateTime.now(),
                  contactName: contactName,
                  contactPhone: '',
                  contactCategory: '',
                  userName: '',
                );
                if (visit == null) {
                  final created = await visitsService.createVisit(visitModel);
                  setState(() => _visits.add(created));
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text('Visita agendada'), backgroundColor: Colors.green),
                  );
                } else {
                  final updated = await visitsService.updateVisit(visitModel);
                  setState(() {
                    final idx = _visits.indexWhere((v) => v.visitId == updated.visitId);
                    if (idx != -1) _visits[idx] = updated;
                  });
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text('Visita actualizada'), backgroundColor: Colors.blue),
                  );
                }
                Navigator.of(context).pop();
              },
              child: Text(visit == null ? 'Agendar' : 'Guardar cambios'),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Agendamiento de Visitas')),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Column(
              children: [
                Expanded(
                  child: ListView.builder(
                    itemCount: _visits.length,
                    itemBuilder: (context, index) {
                      final visit = _visits[index];
                      return Card(
                        child: ListTile(
                          title: Text('Contacto: ${visit.contactName ?? ''}'),
                          subtitle: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('Fecha: ${visit.scheduledDate.toString().split(' ')[0]}'),
                              Text('Hora: ${visit.scheduledDate.hour.toString().padLeft(2, '0')}:${visit.scheduledDate.minute.toString().padLeft(2, '0')}'),
                              Text('Estado: ${visit.status}'),
                              Text('Duración: '), // TODO: Mapear duración
                              Text('Nota: ${visit.notes ?? ''}'),
                            ],
                          ),
                          trailing: IconButton(
                            icon: const Icon(Icons.edit),
                            onPressed: () {
                              _showVisitForm(visit: visit);
                            },
                          ),
                        ),
                      );
                    },
                  ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: ElevatedButton(
                    onPressed: () {
                      _showVisitForm();
                    },
                    child: const Text('Agendar nueva visita'),
                  ),
                ),
              ],
            ),
    );
  }
}

  void _showVisitForm(BuildContext context, {bool isEdit = false}) {
    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: Text(isEdit ? 'Editar Visita' : 'Agendar Visita'),
          content: SingleChildScrollView(
            child: Column(
              children: [
                TextField(
                  decoration: const InputDecoration(labelText: 'Contacto'),
                ),
                TextField(
                  decoration: const InputDecoration(labelText: 'Fecha'),
                ),
                TextField(
                  decoration: const InputDecoration(labelText: 'Hora'),
                ),
                TextField(
                  decoration: const InputDecoration(labelText: 'Estado'),
                ),
                TextField(
                  decoration: const InputDecoration(labelText: 'Duración'),
                ),
                TextField(
                  decoration: const InputDecoration(labelText: 'Nota'),
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancelar'),
            ),
            ElevatedButton(
              onPressed: () {
                // TODO: Guardar visita
                Navigator.of(context).pop();
              },
              child: Text(isEdit ? 'Guardar cambios' : 'Agendar'),
            ),
          ],
        );
      },
    );
  }
}
