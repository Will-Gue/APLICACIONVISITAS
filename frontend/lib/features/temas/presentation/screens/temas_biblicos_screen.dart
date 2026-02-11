import 'package:flutter/material.dart';

class TemasBiblicosScreen extends StatelessWidget {
  const TemasBiblicosScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Temas Bíblicos'),
      ),
      body: ListView.builder(
        itemCount: 5, // TODO: Reemplazar por lista real de temas
        itemBuilder: (context, index) {
          return Card(
            child: ListTile(
              title: Text('Tema Bíblico $index'),
              subtitle: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: const [
                  Text('Categoría: General'),
                  Text('Duración: 30 min'),
                ],
              ),
              trailing: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  IconButton(
                    icon: const Icon(Icons.picture_as_pdf),
                    onPressed: () {
                      // TODO: Visualizar PDF
                    },
                  ),
                  IconButton(
                    icon: const Icon(Icons.download),
                    onPressed: () {
                      // TODO: Descargar PDF
                    },
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
