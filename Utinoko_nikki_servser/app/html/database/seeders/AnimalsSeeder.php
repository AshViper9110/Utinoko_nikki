<?php


namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class AnimalsSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('animals')->insert([
            [
                'name' => 'トイプードル',
            ],
        ]);
    }
}
