#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QImage>
#include <QPainter>
#include <QStaticText>
#include <QFileDialog>
#include "mywidget.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

QImage *buf, *bmp;

void MainWindow::on_pushButton_clicked()
{
    QString fname = QFileDialog::getOpenFileName( this );

    bmp = new QImage();
    bmp->load( fname );

    // В черно-белое
    for( int y = 0; y < bmp->height(); y++ )
    {
        for( int x = 0; x < bmp->width(); x++ )
        {
            QRgb c = bmp->pixel( x, y );

            int r, g, b, t;

            r = c & 0xFF;
            g = (c >> 8) & 0xFF;
            b = (c >> 16) & 0xFF;

            t = (r + g + b) / 3;
            if (t > 255) t = 255;

            bmp->setPixel( x, y, t + (t << 8) + (t << 16) + (0xFF << 24) );
        }
    } // */

    ui->widget->repaint();

    delete buf;
    buf = NULL;
}

void MyWidget::paintEvent(QPaintEvent *event)
{
    QPainter p(this);


    if (bmp == NULL)
    {
        p.drawStaticText( 10, 10, QStaticText( "Не выбрана картинка" ) );
        return;
    }

    if (buf == NULL)
        p.drawImage( 0,0, *bmp );
    else
        p.drawImage( 0,0, *buf );
}

void MainWindow::on_pushButton_2_clicked()
{
    if (bmp == NULL) return;

    if (buf != NULL)
    {
        delete buf;
        buf = NULL;
    }

    int stat[256];

    for( int i = 0; i < 256; i++ )
        stat[i] = 0;

    for( int y = 0; y < bmp->height(); y++ )
    {
        for( int x = 0; x < bmp->width(); x++ )
        {
            QRgb c = bmp->pixel( x, y );

            int r;

            r = c & 0xFF;

            stat[r]++;
        }
    }

    int min, max, s, size;

    s = 0;
    size = bmp->width() * bmp->height();

    for( min = 0; min < 255; min++)
    {
        s += stat[min];

        if (s > size * 0.01f)
            break;
    }

    s = 0;
    for( max = 255; max > 0; max-- )
    {
        s += stat[max];

        if (s > size * 0.01f)
            break;
    }

    ui->label->setText( "Черная точка " + QString::number(min) +
                        " и белая точка " + QString::number(max) );

    int cnv[256];

    for( int i = 0; i < 256; i++ )
    {
        if (i < min)
            cnv[i] = 0;
        else if (i >= max)
            cnv[i] = 255;
        else
        {
            cnv[i] = 255.0f * (i - min) / (max-min);
        }
    }
        // cnv[i] = (max -   min) * i / 255.0f + min;

    for( int y = 0; y < bmp->height(); y++ )
    {
        for( int x = 0; x < bmp->width(); x++ )
        {
            QRgb c = bmp->pixel( x, y );

            int r;

            r = c & 0xFF;
            r = cnv[r];

            bmp->setPixel( x, y, r + (r << 8) + (r << 16) + (0xFF << 24) );
        }
    }

    ui->widget->repaint();
}

typedef int Matr3[3][3];

Matr3 matrices[] =
{
    {
        { -1, -1, -1 },
        { -1,  8, -1 },
        { -1, -1, -1 }
    },
    {
        { -1, -1, -1 },
        {  2,  2,  2 },
        { -1, -1, -1 }
    },
    {
        { -1,  2, -1 },
        { -1,  2, -1 },
        { -1, 2, -1 }
    },
    {
        { 2, -1, -1 },
        { -1,  2, -1 },
        { -1, -1, 2 }
    },
    {
        { -1, -1, 2 },
        { -1,  2, -1 },
        { 2, -1, -1 }
    },
};

void MainWindow::on_pushButton_3_clicked()
{
    if (bmp == NULL) return;

    if (buf == NULL)
        buf = new QImage( bmp->width(), bmp->height(), QImage::Format_RGB32 ); // */

    Matr3 m;

    if (ui->radioButton->isChecked())
        memcpy( &m, &matrices[0], sizeof(Matr3) );
    if (ui->radioButton_2->isChecked())
        memcpy( &m, &matrices[1], sizeof(Matr3) );
    if (ui->radioButton_3->isChecked())
        memcpy( &m, &matrices[2], sizeof(Matr3) );
    if (ui->radioButton_4->isChecked())
        memcpy( &m, &matrices[3], sizeof(Matr3) );
    if (ui->radioButton_5->isChecked())
        memcpy( &m, &matrices[4], sizeof(Matr3) );

    for( int y = 1; y < bmp->height()-1; y++ )
    {
        for( int x = 1; x < bmp->width()-1; x++ )
        {
            int s = 0;

            for( int i = 0; i < 3; i++ )
                for( int j = 0; j < 3; j++ )
                {
                    QRgb c = bmp->pixel( x + j-1, y + i-1 );

                    int r;

                    r = c & 0xFF;

                    s += m[i][j] * r;
                }

            if (s < 0) s = 0;
            if (s > 255) s = 255;

            buf->setPixel( x, y, s + (s << 8) + (s << 16) + (0xFF << 24) );
        }
    }

    ui->widget->repaint();
}
